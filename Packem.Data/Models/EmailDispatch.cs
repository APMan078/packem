using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace Packem.Data.Models
{
    public class EmailDispatch
    {
        private string _url;
        private string _name;

        public EmailDispatch(string name, string url)
        {
            _name = name;
            _url = url;
        }

        public AlternateView PasswordResetTemplate { get { return GetPasswordResetTemplate(); } }

        private AlternateView GetPasswordResetTemplate()
        {
            var path = Path.GetFullPath("./Assets/logo.png").Replace("~\\", "");
            LinkedResource linkedImage = new LinkedResource(path);
            linkedImage.ContentId = "Logo";
            linkedImage.ContentType = new ContentType(MediaTypeNames.Image.Jpeg);

            var _template = File.ReadAllText(@"HTMLEmailTemplates/PasswordResetTemplate.html");
            _template = _template.Replace("Url", _url);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
              _template,
              null, MediaTypeNames.Text.Html);

            htmlView.LinkedResources.Add(linkedImage);
            return htmlView;
        }
    }
}

