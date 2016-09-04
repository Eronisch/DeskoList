using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace elFinder.Connector.Service
{
	public class LocalFileSystemVolume : IVolume
	{
		private readonly Config.IConnectorConfig _config;
		private readonly ICryptoService _cryptoService;
		private readonly IImageEditorService _imageEditorService;

		public LocalFileSystemVolume( Config.IConnectorConfig config, ICryptoService cryptoService,
			IImageEditorService imageEditorService )
		{
			_config = config;
			_cryptoService = cryptoService;
			_imageEditorService = imageEditorService;
		}

		#region IVolume Members

		public string Id
		{
			get;
			set;
		}

		public string Name
		{
			get { return "LocalFileSystem"; }
		}

		public bool IsHashFromThisVolume( string hashToCheck )
		{
			if( string.IsNullOrWhiteSpace( hashToCheck ) )
				return false;
			return hashToCheck.StartsWith( Id, StringComparison.OrdinalIgnoreCase );
		}

		private DirectoryInfo getValidDirectoryInfo( string absolutePath )
		{
			DirectoryInfo di = new DirectoryInfo( absolutePath );
			if( !isValidDirectoryInfo( di ) )
				return null;
			return di;
		}

		private FileInfo getValidFileInfo( string absolutePath )
		{
			FileInfo fi = new FileInfo( absolutePath );
			if( !isValidFileInfo( fi ) )
				return null;
			return fi;
		}

		private bool isValidDirectoryInfo( DirectoryInfo di )
		{
			// first check if this directory isn't higher than our root
			if( di.FullName.Length < _config.LocalFSRootDirectoryPath.Length || !di.FullName.StartsWith( _config.LocalFSRootDirectoryPath, StringComparison.OrdinalIgnoreCase ) )
				return false;
			return ( di != null && di.Exists && di.Attributes.HasFlag( FileAttributes.Directory )
				&& !di.Attributes.HasFlag( FileAttributes.Hidden )
				&& !di.Attributes.HasFlag( FileAttributes.System ) ); // dont want hidden directories or files
		}

		private bool isValidFileInfo( FileInfo fi )
		{
			return ( fi != null && fi.Exists
				&& !fi.Attributes.HasFlag( FileAttributes.Hidden )
				&& !fi.Attributes.HasFlag( FileAttributes.System ) ); // dont want hidden directories or files
		}

		private Model.DirectoryModel createDirectoryModel( DirectoryInfo di, string directoryHash, string parentHash )
		{
			// check if path is root path - if yes then we need to change name
			string dirName = di.Name;
			if( di.FullName == _config.LocalFSRootDirectoryPath )
				dirName = _config.RootDirectoryName;
			return new Model.DirectoryModel( dirName, directoryHash, parentHash,
				di.GetDirectories().Length > 0, di.LastWriteTime, Id, true,
				!di.Attributes.HasFlag( FileAttributes.ReadOnly ), false );
		}

		private Model.FileModel createFileModel( FileInfo fi, string parentHash )
		{
			string hash = EncodePathToHash( fi.FullName );
			string thumbnail = null;
			// supports thumbnails?
			if( _imageEditorService.CanGenerateThumbnail( fi.FullName ) )
			{
				if( !_config.LocalFSThumbsDirectoryPath.Contains( fi.DirectoryName ) )
				{
					// check if thumbnail exists - cache it maybe?
					string thumbnailPath = _config.LocalFSThumbsDirectoryPath + Path.DirectorySeparatorChar + hash + fi.Extension;
					if( File.Exists( thumbnailPath ) )
						thumbnail = hash + fi.Extension;
					else
						thumbnail = Model.FileModel.ThumbnailIsSupported; // can create thumbnail
				}
			}
			return new Model.FileModel( fi.Name, thumbnail, hash,
						fi.Length, parentHash, fi.LastWriteTime, Id,
						true, !fi.IsReadOnly, false );
		}

		public Model.DirectoryModel GetDirectoryByHash( string directoryHash )
		{
			// decrypt to get real path
			// check if passed hash starts with our id
			if( !IsHashFromThisVolume( directoryHash ) )
				return null;
			string absolutePath = DecodeHashToPath( directoryHash, true );
			// finally we can get some info
			DirectoryInfo di = getValidDirectoryInfo( absolutePath );
			if( di == null )
				return null; // dont want hidden directories

			string parentHash = null;
			if( isValidDirectoryInfo( di.Parent ) )
			{
				// check if parent path ends with separator - it is required
				string parentPath = di.Parent.FullName;
				if( parentPath.Last() != Path.DirectorySeparatorChar )
					parentPath += Path.DirectorySeparatorChar;
				parentHash = EncodePathToHash( parentPath );
			}

			return createDirectoryModel( di, directoryHash, parentHash );
		}

		public Model.FileModel GetFileByHash( string fileHash )
		{
			if( !IsHashFromThisVolume( fileHash ) )
				return null;

			string absolutePath = DecodeHashToPath( fileHash, true );
			var fi = getValidFileInfo( absolutePath );
			if( fi == null )
				return null;
			// validate directory also
			var fileDirectory = fi.Directory;
			if( !isValidDirectoryInfo( fileDirectory ) )
				return null;

			string parentPath = fileDirectory.FullName;
			if( parentPath.Last() != Path.DirectorySeparatorChar )
				parentPath += Path.DirectorySeparatorChar;

			return createFileModel( fi, EncodePathToHash( parentPath ) );
		}

		public Model.DirectoryModel GetRootDirectory()
		{
			string absolutePath = _config.LocalFSRootDirectoryPath;
			DirectoryInfo di = getValidDirectoryInfo( absolutePath );
			if( di == null )
				return null;

			return createDirectoryModel( di, EncodePathToHash( di.FullName ), null );
		}

		private void getSubDirs( string rootDir, string rootDirHash, List<Model.DirectoryModel> subDirs, 
			int maxTreeLevel, int level )
		{
			if( level > maxTreeLevel )
				return;

			string[] dirs = Directory.GetDirectories( rootDir );
			foreach( var dir in dirs )
			{
				DirectoryInfo di = getValidDirectoryInfo( dir );
				if( di == null )
					continue;
				string hash = EncodePathToHash( di.FullName );

				subDirs.Add( createDirectoryModel( di, hash, rootDirHash ) );
				getSubDirs( di.FullName, hash, subDirs, maxTreeLevel, level + 1 );
			}
		}

		public IEnumerable<Model.DirectoryModel> GetSubdirectoriesFlat( Model.DirectoryModel rootDirectory, int? maxDepth = null )
		{
			if( rootDirectory == null )
				return new List<Model.DirectoryModel>();

			string dirPath = DecodeHashToPath( rootDirectory.Hash );
			List<Model.DirectoryModel> subDirs = new List<Model.DirectoryModel>();
			try
			{
				getSubDirs( dirPath, rootDirectory.Hash, subDirs, maxDepth ?? _config.MaxTreeLevel, 0 );
			}
			catch
			{
				return new List<Model.DirectoryModel>();
			}
			return subDirs;
		}

		public IEnumerable<Model.FileModel> GetFiles( Model.DirectoryModel rootDirectory )
		{
			if( rootDirectory == null )
				return new List<Model.FileModel>();

			string dirPath = DecodeHashToPath( rootDirectory.Hash );
			string[] files = Directory.GetFiles( dirPath );
			var filesModels = files.Select( x =>
				{
					var fi = getValidFileInfo( x );
					if( fi == null )
						return null;
					return createFileModel( fi, rootDirectory.Hash );
				} ).Where( x => x != null );
			return filesModels;
		}

		public string GetPathToRoot( Model.DirectoryModel startDir )
		{
			StringBuilder sb = new StringBuilder( startDir.Name );
			var rootDir = GetRootDirectory();
			var currDir = startDir;
			while( currDir != null && currDir.Hash != rootDir.Hash )
			{
				if( currDir.ParentHash == null )
					break;
				var parentDir = GetDirectoryByHash( currDir.ParentHash );
				if( parentDir == null )
					break;
				// add our root dir
				sb.Insert( 0, Path.DirectorySeparatorChar + parentDir.Name + Path.DirectorySeparatorChar );
				currDir = parentDir;
			}
			// trim separator from begining
			if( sb[ 0 ] == Path.DirectorySeparatorChar )
				sb.Remove( 0, 1 );
			return sb.ToString();
		}

		public Model.DirectoryModel CreateDirectory( Model.DirectoryModel inDir, string name )
		{
			if( inDir == null )
				return null;
			// compose final path of new directory
			string path = DecodeHashToPath( inDir.Hash );
			if( path.Last() != Path.DirectorySeparatorChar )
				path += Path.DirectorySeparatorChar;
			path += name;
			// check if directory exists
			if( Directory.Exists( path ) )
				return null;

			try
			{
				var createdDirectory = Directory.CreateDirectory( path );
				return createDirectoryModel( createdDirectory, EncodePathToHash( path ), inDir.Hash );
			}
			catch
			{
				return null;
			}
		}

		public Model.FileModel CreateFile( Model.DirectoryModel inDir, string name )
		{
			if( inDir == null )
				return null;
			// compose final path of new directory
			string path = DecodeHashToPath( inDir.Hash );
			if( path.Last() != Path.DirectorySeparatorChar )
				path += Path.DirectorySeparatorChar;
			path += name;
			// check if file exists
			if( File.Exists( path ) )
				return null;

			try
			{
				using( var createdFile = File.Create( path ) ) { }
				FileInfo fi = new FileInfo( path );
				return createFileModel( fi, inDir.Hash );
			}
			catch
			{
				return null;
			}
		}

		public Model.FileModel RenameFile( Model.FileModel fileToChange, string newname )
		{
			if( fileToChange == null )
				return null;

			string path = DecodeHashToPath( fileToChange.Hash );
			FileInfo fi = new FileInfo( path );

			string parentDir = fi.Directory.FullName;
			if( parentDir.Last() != Path.DirectorySeparatorChar )
				parentDir += Path.DirectorySeparatorChar;
			string newPath = parentDir + newname;

			if( File.Exists( newPath ) )
				return null;
			try
			{
				File.Move( path, newPath );
				FileInfo newFileInfo = new FileInfo( newPath );
				return createFileModel( newFileInfo, EncodePathToHash( parentDir ) );
			}
			catch
			{
				return null;
			}
		}

		public Model.DirectoryModel RenameDirectory( Model.DirectoryModel dirToChange, string newname )
		{
			if( dirToChange == null )
				return null;

			string path = DecodeHashToPath( dirToChange.Hash );
			DirectoryInfo di = new DirectoryInfo( path );
			if( di.Parent == null )
				return null;

			string parentDir = di.Parent.FullName;
			if( parentDir.Last() != Path.DirectorySeparatorChar )
				parentDir += Path.DirectorySeparatorChar;
			string newPath = parentDir + newname;

			if( Directory.Exists( newPath ) )
				return null;

			try
			{
				Directory.Move( path, newPath );
				DirectoryInfo newDirInfo = new DirectoryInfo( newPath );
				return createDirectoryModel( newDirInfo, EncodePathToHash( newDirInfo.FullName ),
					EncodePathToHash( parentDir ) );
			}
			catch
			{
				return null;
			}
		}

		public bool DeleteFile( Model.FileModel fileToDelete )
		{
			if( fileToDelete == null )
				return false;

			string path = DecodeHashToPath( fileToDelete.Hash );
			if( !File.Exists( path ) )
				return false;

			try
			{
				File.Delete( path );
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool DeleteThumbnailFor( Model.FileModel deleteThumbnailForFile )
		{
			if( deleteThumbnailForFile == null )
				return false;
			// get file extension and compose thumbnail path
			FileInfo fi = new FileInfo( deleteThumbnailForFile.Name );
			string thumbnailPath = _config.LocalFSThumbsDirectoryPath + Path.DirectorySeparatorChar
				+ deleteThumbnailForFile.Hash + fi.Extension;
			if( !File.Exists( thumbnailPath ) )
				return false;

			try
			{
				File.Delete( thumbnailPath );
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool DeleteDirectory( Model.DirectoryModel directoryToDelete )
		{
			if( directoryToDelete == null )
				return false;

			string path = DecodeHashToPath( directoryToDelete.Hash );
			if( !Directory.Exists( path ) )
				return false;

			try
			{
				Directory.Delete( path, true );
				return true;
			}
			catch
			{
				return false;
			}
		}

		public Model.FileModel CopyFile( Model.FileModel fileToCopy, string destinationDirectory, 
			bool cut )
		{
			if( fileToCopy == null || string.IsNullOrWhiteSpace( destinationDirectory ) )
				return null;
			if( !Directory.Exists( destinationDirectory ) )
				return null;

			string path = DecodeHashToPath( fileToCopy.Hash );
			FileInfo fi = new FileInfo( path );
			// compose final directory
			string destDir = destinationDirectory;
			if( destDir.Last() != Path.DirectorySeparatorChar )
				destDir += Path.DirectorySeparatorChar;
			string newPath = destDir + fi.Name;

			if( File.Exists( newPath ) )
				return null;

			try
			{
				// check for cut or copy
				if( cut )
					File.Move( path, newPath );
				else
					File.Copy( path, newPath );

				FileInfo newFileInfo = new FileInfo( newPath );
				return createFileModel( newFileInfo, EncodePathToHash( destDir ) );
			}
			catch
			{
				return null;
			}
		}

		public Model.DirectoryModel CopyDirectory( Model.DirectoryModel directoryToCopy, string destinationDirectory,
			bool cut )
		{
			if( directoryToCopy == null || string.IsNullOrWhiteSpace( destinationDirectory ) )
				return null;
			if( !Directory.Exists( destinationDirectory ) )
				return null;

			string path = DecodeHashToPath( directoryToCopy.Hash );
			DirectoryInfo di = new DirectoryInfo( path );
			// compose final directory
			string destDir = destinationDirectory;
			if( destDir.Last() != Path.DirectorySeparatorChar )
				destDir += Path.DirectorySeparatorChar;
			string newPath = destDir + di.Name;

			if( Directory.Exists( newPath ) )
				return null;

			try
			{
				if( cut )
					Directory.Move( path, newPath );
				else
					copyDirectory( path, newPath, true );
				// get new parent dir
				DirectoryInfo newDirInfo = new DirectoryInfo( newPath );
				if( newDirInfo.Parent == null )
					return null;
				string parentDir = newDirInfo.Parent.FullName;
				if( parentDir.Last() != Path.DirectorySeparatorChar )
					parentDir += Path.DirectorySeparatorChar;

				return createDirectoryModel( newDirInfo, EncodePathToHash( newDirInfo.FullName ),
					EncodePathToHash( parentDir ) );
			}
			catch
			{
				return null;
			}
		}

		private static void copyDirectory( string sourceDirName, string destDirName, bool copySubDirs )
		{
			DirectoryInfo dir = new DirectoryInfo( sourceDirName );
			DirectoryInfo[] dirs = dir.GetDirectories();

			if( !dir.Exists )
				return;

			if( !Directory.Exists( destDirName ) )
				Directory.CreateDirectory( destDirName );

			FileInfo[] files = dir.GetFiles();
			foreach( FileInfo file in files )
			{
				string temppath = Path.Combine( destDirName, file.Name );
				file.CopyTo( temppath, false );
			}

			if( copySubDirs )
			{
				foreach( DirectoryInfo subdir in dirs )
				{
					string temppath = Path.Combine( destDirName, subdir.Name );
					copyDirectory( subdir.FullName, temppath, copySubDirs );
				}
			}
		}

		public Model.FileModel DuplicateFile( Model.FileModel fileToDuplicate )
		{
			if( fileToDuplicate == null )
				return null;

			string path = DecodeHashToPath( fileToDuplicate.Hash );

			FileInfo fi = new FileInfo( path );
			// compose final file path
			string destDir = fi.DirectoryName;
			if( destDir.Last() != Path.DirectorySeparatorChar )
				destDir += Path.DirectorySeparatorChar;
			string newPath = destDir + string.Format( _config.DuplicateFilePattern, fi.Name, fi.Extension );
			// new file should be here
			if( File.Exists( newPath ) )
				return null;

			try
			{
				File.Copy( path, newPath );

				FileInfo newFileInfo = new FileInfo( newPath );
				return createFileModel( newFileInfo, EncodePathToHash( destDir ) );
			}
			catch
			{
				return null;
			}
		}

		public Model.DirectoryModel DuplicateDirectory( Model.DirectoryModel directoryToDuplicate )
		{
			if( directoryToDuplicate == null )
				return null;

			string path = DecodeHashToPath( directoryToDuplicate.Hash );
			DirectoryInfo di = new DirectoryInfo( path );
			// compose final directory path
			string destDir = di.Parent.FullName;
			if( destDir.Last() != Path.DirectorySeparatorChar )
				destDir += Path.DirectorySeparatorChar;
			string newPath = destDir + string.Format( _config.DuplicateDirectoryPattern, di.Name );
			// new directory shouldn't be here
			if( Directory.Exists( newPath ) )
				return null;

			try
			{
				copyDirectory( path, newPath, true );
				// get new parent dir
				DirectoryInfo newDirInfo = new DirectoryInfo( newPath );
				if( newDirInfo.Parent == null )
					return null;
				string parentDir = newDirInfo.Parent.FullName;
				if( parentDir.Last() != Path.DirectorySeparatorChar )
					parentDir += Path.DirectorySeparatorChar;

				return createDirectoryModel( newDirInfo, EncodePathToHash( newDirInfo.FullName ),
					EncodePathToHash( parentDir ) );
			}
			catch
			{
				return null;
			}
		}

		public string GetTextFileContent( Model.FileModel fileToGet )
		{
			if( fileToGet == null )
				return null;

			string path = DecodeHashToPath( fileToGet.Hash );
			try
			{
				return File.ReadAllText( path );
			}
			catch
			{
				return null;
			}
		}

		public Model.FileModel SetTextFileContent( Model.FileModel fileToModify, string content )
		{
			if( fileToModify == null )
				return null;

			string path = DecodeHashToPath( fileToModify.Hash );
			if( !File.Exists( path ) )
				return null;

			try
			{
				File.WriteAllText( path, content );
				return createFileModel( new FileInfo( path ), fileToModify.ParentHash );
			}
			catch
			{
				return null;
			}
		}

		public Model.FileModel[] SaveFiles( string targetDirHash, IList<HttpPostedFile> files )
		{
			if( files == null )
				return new Model.FileModel[ 0 ];

			string targetPath = DecodeHashToPath( targetDirHash );
			if( !Directory.Exists( targetPath ) )
				return new Model.FileModel[ 0 ];

			if( targetPath.Last() != Path.DirectorySeparatorChar )
				targetPath += Path.DirectorySeparatorChar;

			IList<Model.FileModel> added = new List<Model.FileModel>();
			for( int i = 0; i < files.Count; ++i )
			{
				HttpPostedFile file = files[ i ];
				// handle special upload case, when "Include local directory path when uploading files" in IE is enabled (this might be when in intranet site):
				// http://blogs.msdn.com/b/webtopics/archive/2009/07/27/uploading-a-file-using-fileupload-control-fails-in-ie8.aspx
				string fName = Path.GetFileName( file.FileName );
				string filePath = targetPath + fName;
				try
				{
					file.SaveAs( filePath );
					FileInfo fi = new FileInfo( filePath );
					added.Add( createFileModel( fi, targetDirHash ) );
				}
				catch
				{
					continue;
				}
			}
			return added.ToArray();
		}

		public string EncodePathToHash( string path, bool fromAbsolutePath = true )
		{
			string relativePath = path;
			if( fromAbsolutePath )
			{
				if( path.Length < _config.LocalFSRootDirectoryPath.Length )
					throw new InvalidOperationException( "Cannot convert absolute path: " 
						+ path + " to relative to: " + _config.LocalFSRootDirectoryPath );
				// convert from absolute to relative
				relativePath = path.Substring( _config.LocalFSRootDirectoryPath.Length );
				if( string.IsNullOrWhiteSpace( relativePath ) )
					relativePath = "";
			}
			relativePath = relativePath.TrimEnd( Path.DirectorySeparatorChar );
			string encodedPath = _cryptoService.Encode( relativePath );
			// prepend with id
			return Id + encodedPath;
		}

		public string DecodeHashToPath( string hash, bool toAbsolutePath = true )
		{
			// remove volume id from the beginig
			string fixedHash = hash.Substring( Id.Length );
			// and remove trailing extension if exists
			int lastDot = fixedHash.LastIndexOf( '.' );
			if( lastDot >= 0 )
				fixedHash = fixedHash.Substring( 0, lastDot );
			// decode hash to real object name
			string objectRelativePath = _cryptoService.Decode( fixedHash );
			if( !toAbsolutePath )
				return objectRelativePath;
			// now get absolute path
			string absolutePath = _config.LocalFSRootDirectoryPath;
			if( !string.IsNullOrWhiteSpace(objectRelativePath) 
				&& objectRelativePath[ 0 ] != Path.DirectorySeparatorChar 
				&& absolutePath.Last() != Path.DirectorySeparatorChar )
				absolutePath += Path.DirectorySeparatorChar;
			if( absolutePath.Last() == Path.DirectorySeparatorChar && !string.IsNullOrEmpty(objectRelativePath) && objectRelativePath[ 0 ] == Path.DirectorySeparatorChar )
				// need to remove one of the separators
				objectRelativePath = objectRelativePath.Substring( 1 );
			absolutePath += objectRelativePath;
			return absolutePath;
		}

		#endregion
	}
}
