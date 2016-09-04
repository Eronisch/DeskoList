using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace elFinder.Connector.Service
{
	public interface IVolume
	{
		string Id { get; set; }

		string Name { get; }

		Model.DirectoryModel GetDirectoryByHash( string directoryHash );
		Model.DirectoryModel GetRootDirectory();

		IEnumerable<Model.DirectoryModel> GetSubdirectoriesFlat( Model.DirectoryModel rootDirectory, int? maxDepth = null );

		IEnumerable<Model.FileModel> GetFiles( Model.DirectoryModel rootDirectory );

		string GetPathToRoot( Model.DirectoryModel startDir );

		string EncodePathToHash( string path, bool fromAbsolutePath = true );
		string DecodeHashToPath( string hash, bool toAbsolutePath = true );

		Model.DirectoryModel CreateDirectory( Model.DirectoryModel cwd, string name );
		Model.FileModel CreateFile( Model.DirectoryModel inDir, string name );

		Model.FileModel GetFileByHash( string fileHash );

		Model.FileModel RenameFile( Model.FileModel fileToChange, string newname );
		Model.DirectoryModel RenameDirectory( Model.DirectoryModel dirToChange, string newname );

		bool DeleteFile( Model.FileModel fileToRemove );
		bool DeleteDirectory( Model.DirectoryModel directoryToRemove );
		bool DeleteThumbnailFor( Model.FileModel deleteThumbnailForFile );

		Model.FileModel CopyFile( Model.FileModel fileToCopy, string destinationDirectory, bool cut );
		Model.DirectoryModel CopyDirectory( Model.DirectoryModel directoryToCopy, string destinationDirectory, bool cut );

		Model.FileModel DuplicateFile( Model.FileModel fileToDuplicate );
		Model.DirectoryModel DuplicateDirectory( Model.DirectoryModel directoryToDuplicate );

		Model.FileModel[] SaveFiles( string targetDirHash, IList<HttpPostedFile> files );

		string GetTextFileContent( Model.FileModel fileToGet );
		Model.FileModel SetTextFileContent( Model.FileModel fileToModify, string content );
	}
}
