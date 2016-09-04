using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace elFinder.Connector.Service
{
	public class DefaultImageEditorService : IImageEditorService
	{
		private static object _mutex = new object();
		private IList<string> _supportedExtensions;

		#region IThumbnailerService Members

		public IEnumerable<string> SupportedExtensions
		{
			get
			{
				if( _supportedExtensions == null )
				{
					lock( _mutex )
					{
						// cache supported extensions
						if( _supportedExtensions == null )
							_supportedExtensions = ImageCodecInfo.GetImageEncoders().SelectMany(
								x => x.FilenameExtension.Replace( "*.", string.Empty ).ToLowerInvariant().Split( ';' ) ).ToList();
					}
				}
				return _supportedExtensions;
			}
		}

		public bool CanGenerateThumbnail( string filePath )
		{
			string fileExt = Path.GetExtension( filePath ).ToLowerInvariant().Trim( '.' );
			return SupportedExtensions.Contains( fileExt );
		}

		private ImageCodecInfo getEncoder( string fileExt, out EncoderParameters encParams )
		{
			// setup standard params (high quality = 90%)
			encParams = new EncoderParameters( 1 );
			encParams.Param[ 0 ] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 90L );

			return ImageCodecInfo.GetImageEncoders().First( info =>
						info.FilenameExtension.IndexOf( fileExt, StringComparison.OrdinalIgnoreCase ) > -1 );
		}

		//TODO handle error logging because exceptions might be tricky to find otherwise
		public string CreateThumbnail( string sourceImagePath, string destThumbsDir, string thumbFileName,
			System.Drawing.Size thumbSize, bool restrictWidth )
		{
			try
			{
				string fileExt = Path.GetExtension( sourceImagePath ).ToLowerInvariant().Trim( '.' );

				if( destThumbsDir.Last() != Path.DirectorySeparatorChar )
					destThumbsDir += Path.DirectorySeparatorChar;

				string outputFileName = thumbFileName + "." + fileExt;
				string outputPath = destThumbsDir + outputFileName;

				if( File.Exists( outputPath ) )
					return null;

				using( Bitmap inputImage = new Bitmap( sourceImagePath ) )
				{
					EncoderParameters encParams = null;
					var encoder = getEncoder( fileExt, out encParams );

					try
					{
						if( restrictWidth )
						{
							float scale = thumbSize.Width / (float)inputImage.Width;
							Size outSize = new Size( thumbSize.Width, (int)Math.Floor( inputImage.Height * scale ) );
							using( Bitmap outBmp = new Bitmap( outSize.Width, outSize.Height ) )
							{
								using( Graphics g = Graphics.FromImage( outBmp ) )
								{
									g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
									g.DrawImage( inputImage, 0, 0, outSize.Width, outSize.Height );
								}
								outBmp.Save( outputPath, encoder, encParams );
							}
						}
						else
						{
							// first rescale image to aspect ratio
							float wFactor = inputImage.Width / (float)thumbSize.Width;
							float hFactor = inputImage.Height / (float)thumbSize.Height;
							float minFactor = Math.Min( wFactor, hFactor );
							Size tempSize = new Size( (int)Math.Round( thumbSize.Width * minFactor ),
										(int)Math.Round( thumbSize.Height * minFactor ) );

							Rectangle clipRectangle = new Rectangle(
										( inputImage.Width - tempSize.Width ) / 2,
									   ( inputImage.Height - tempSize.Height ) / 2,
									   tempSize.Width, tempSize.Height );

							using( Bitmap tempBmp = new Bitmap( tempSize.Width, tempSize.Height ) )
							{
								// clip image
								tempBmp.SetResolution( inputImage.HorizontalResolution, inputImage.VerticalResolution );
								using( Graphics g = Graphics.FromImage( tempBmp ) )
								{
									g.DrawImage( inputImage, 0, 0, clipRectangle, GraphicsUnit.Pixel );
								}
								// resize bitmap
								using( Bitmap resBitmap = new Bitmap( thumbSize.Width, thumbSize.Height ) )
								{
									using( Graphics g = Graphics.FromImage( resBitmap ) )
									{
										g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
										g.DrawImage( tempBmp, 0, 0, thumbSize.Width, thumbSize.Height );
									}
									resBitmap.Save( outputPath, encoder, encParams );
								}
							}
						}
					}
					catch
					{
						return null;
					}

				}
				return outputFileName;
			}
			catch
			{
				return null;
			}
		}

		private string getTempFileName( string sourceImagePath, string fileExt )
		{
			return Path.GetDirectoryName( sourceImagePath ) + Path.DirectorySeparatorChar
					+ Guid.NewGuid() + "." + fileExt;
		}

		public bool CropImage( string sourceImagePath, Point topLeft, Size newSize )
		{
			try
			{
				string fileExt = Path.GetExtension( sourceImagePath ).ToLowerInvariant().Trim( '.' );
				// create file name of the temp file
				string tempFilePath = getTempFileName( sourceImagePath, fileExt );

				using( Bitmap inputImage = new Bitmap( sourceImagePath ) )
				{
					EncoderParameters encParams = null;
					var encoder = getEncoder( fileExt, out encParams );

					using( Bitmap tempBmp = new Bitmap( newSize.Width, newSize.Height ) )
					{
						// clip image
						tempBmp.SetResolution( inputImage.HorizontalResolution, inputImage.VerticalResolution );
						using( Graphics g = Graphics.FromImage( tempBmp ) )
						{
							g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
							g.DrawImage( inputImage, 0, 0, new Rectangle( topLeft, newSize ), GraphicsUnit.Pixel );
						}
						tempBmp.Save( tempFilePath, encoder, encParams );
					}
				}
				// now replace images
				File.Delete( sourceImagePath );
				File.Move( tempFilePath, sourceImagePath );
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool ResizeImage( string sourceImagePath, Size newSize )
		{
			try
			{
				string fileExt = Path.GetExtension( sourceImagePath ).ToLowerInvariant().Trim( '.' );
				// create file name of the temp file
				string tempFilePath = getTempFileName( sourceImagePath, fileExt );

				using( Bitmap inputImage = new Bitmap( sourceImagePath ) )
				{
					EncoderParameters encParams = null;
					var encoder = getEncoder( fileExt, out encParams );

					using( Bitmap tempBmp = new Bitmap( newSize.Width, newSize.Height ) )
					{
						// resize image
						tempBmp.SetResolution( inputImage.HorizontalResolution, inputImage.VerticalResolution );
						using( Graphics g = Graphics.FromImage( tempBmp ) )
						{
							g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
							g.DrawImage( inputImage, new Rectangle( 0, 0, newSize.Width, newSize.Height ) );
						}
						tempBmp.Save( tempFilePath, encoder, encParams );
					}
				}
				// now replace images
				File.Delete( sourceImagePath );
				File.Move( tempFilePath, sourceImagePath );
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool RotateImage( string sourceImagePath, int rotationDegree )
		{
			try
			{
				string fileExt = Path.GetExtension( sourceImagePath ).ToLowerInvariant().Trim( '.' );
				// create file name of the temp file
				string tempFilePath = getTempFileName( sourceImagePath, fileExt );

				using( Bitmap inputImage = new Bitmap( sourceImagePath ) )
				{
					EncoderParameters encParams = null;
					var encoder = getEncoder( fileExt, out encParams );

					// calculate width and height of the new image
					float iW = (float)inputImage.Width;
					float iH = (float)inputImage.Height;

					Matrix whRotation = new Matrix();
					whRotation.Rotate( rotationDegree );
					// rotate every vertex of our "image rectangle"
					var tmpDims = new PointF[] { new PointF(0,0), new PointF( iW, 0 ), new PointF( iW, iH ), new PointF( 0, iH ) };
					whRotation.TransformVectors( tmpDims );
					// find extends
					iW = Math.Abs( tmpDims.Max( x => x.X ) - tmpDims.Min( x => x.X ) );
					iH = Math.Abs( tmpDims.Max( x => x.Y ) - tmpDims.Min( x => x.Y ) );
					
					using( Bitmap tempBmp = new Bitmap( (int)Math.Ceiling( iW ), (int)Math.Ceiling( iH ) ) )
					{
						// rotate image
						tempBmp.SetResolution( inputImage.HorizontalResolution, inputImage.VerticalResolution );
						using( Graphics g = Graphics.FromImage( tempBmp ) )
						{
							g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
							// rotate at the center
							g.TranslateTransform( tempBmp.Width/2, tempBmp.Height/2 );
							g.RotateTransform( rotationDegree );
							g.TranslateTransform( -tempBmp.Width / 2, -tempBmp.Height / 2 );
							g.DrawImage( inputImage,
								new Point( ( tempBmp.Width - inputImage.Width ) / 2,
									( tempBmp.Height - inputImage.Height ) / 2 ) );
						}
						tempBmp.Save( tempFilePath, encoder, encParams );
					}
				}
				// now replace images
				File.Delete( sourceImagePath );
				File.Move( tempFilePath, sourceImagePath );
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion
	}
}
