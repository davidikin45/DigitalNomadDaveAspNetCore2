using AspNetCore.Base.Data.RepositoryFileSystem.File;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Images
{
    public class ImageTests
    {
        [Fact]
        public void AttributesSave()
        {
            var filePath = Path.GetFullPath(@"image.jpg");
            var imageInfo = new ImageInfo(filePath);

            Assert.Equal("Title", imageInfo.Title);
            Assert.Equal("Comments", imageInfo.Comments);
            Assert.Equal("Author", imageInfo.Author);
            Assert.Equal("Keywords", imageInfo.Keywords);
            Assert.Equal("Subject", imageInfo.Subject);


            imageInfo.Title = "New Title";
            imageInfo.Comments = "New Comments";
            imageInfo.Author = "New Author";
            imageInfo.Keywords = "New Keywords";
            imageInfo.Subject = "New Subject";
            imageInfo.DateTimeCreated = imageInfo.DateTimeCreated.AddMinutes(1);

            imageInfo.GPSLatitudeDegrees = 40.7128;
            imageInfo.GPSLongitudeDegrees = -74.0060;

            imageInfo.Save();

            imageInfo = new ImageInfo(filePath);

            Assert.Equal("New Title", imageInfo.Title);
            Assert.Equal("New Comments", imageInfo.Comments);
            Assert.Equal("New Author", imageInfo.Author);
            Assert.Equal("New Keywords", imageInfo.Keywords);
            Assert.Equal("New Subject", imageInfo.Subject);
            Assert.Equal(55, imageInfo.DateTimeCreated.Minute);

            Assert.Equal(40.7128, imageInfo.GPSLatitudeDegrees.Value);
            Assert.Equal(-74.0060, imageInfo.GPSLongitudeDegrees.Value);
        }
    }
}
