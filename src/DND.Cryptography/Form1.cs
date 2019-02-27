using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Windows.Forms;

namespace DND.Cryptography
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerateRSAKeys_Click(object sender, EventArgs e)
        {
            var result = GenerateRsaKeyPair();
            txtPrivateKey.Text = result.privateKey;
            txtPublicKey.Text = result.publicKey;
        }

        private (string publicKey, string privateKey) GenerateRsaKeyPair()
        {
            RsaKeyPairGenerator rsaGenerator = new RsaKeyPairGenerator();
            rsaGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            var keyPair = rsaGenerator.GenerateKeyPair();

            string publicKey;
            using (TextWriter publicKeyTextWriter = new StringWriter())
            {
                PemWriter pemWriter = new PemWriter(publicKeyTextWriter);
                pemWriter.WriteObject(keyPair.Public);
                pemWriter.Writer.Flush();

                publicKey = publicKeyTextWriter.ToString();
            }

            string privateKey;
            using (TextWriter privateKeyTextWriter = new StringWriter())
            {

                PemWriter pemWriter = new PemWriter(privateKeyTextWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();
                privateKey = privateKeyTextWriter.ToString();
            }

            return (publicKey: publicKey, privateKey: privateKey);
        }

        private void btnSaveRSAPublicKey_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "public.x509.pem";
            // set filters - this can be done in properties as well
            savefile.Filter = "Public Key files (*.pub)|*.pub|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, txtPublicKey.Text);
            }
        }

        private void btnSaveRSAPrivateKey_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "private.rsa.pem";
            // set filters - this can be done in properties as well
            savefile.Filter = "Private Key files (*.key)|*.key|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, txtPrivateKey.Text);
            }
        }
    }
}
