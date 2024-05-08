using System.Windows.Input;
using DevExpress.Drawing;
using DevExpress.Maui.Core;
using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;
using CrmDemo.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace CrmDemo.ViewModels.Orders;

[QueryProperty(nameof(DocumentFullPath), "documentFullPath")]
public class OrderPdfPreviewViewModel : BindableBase {
    #region fields
    ImageSource pdfPreview;
    const string defaultCertificateName = "pfxCertificate.pfx";
    const string defaultCertificatePassword = "123";
    string certificateFullPath;
    string documentFullPath;
    bool isSignatureViewOpened;
    #endregion fields
    #region properties
    public string DocumentFullPath {
        get {
            return documentFullPath;
        }
        set {
            documentFullPath = value;
            RaisePropertyChanged(nameof(DocumentFullPath));
            UpdatePreview();
        }
    }

    public ImageSource PdfPreview {
        get {
            return pdfPreview;
        }
        set {
            pdfPreview = value;
            RaisePropertyChanged(nameof(PdfPreview));
        }
    }
    public bool IsSignatureViewOpened {
        get {
            return isSignatureViewOpened;
        }
        set {
            isSignatureViewOpened = value;
            RaisePropertyChanged(nameof(IsSignatureViewOpened));
        }
    }
    public ICommand SignPdfCommand { get; set; }
    public ICommand SharePdfCommand { get; set; }
    public ICommand OpenFileCommand { get; set; }
    public ICommand OpenSignatureViewCommand { get; set; }
    public ICommand CloseSignatureViewCommand { get; set; }
    #endregion properties
    public OrderPdfPreviewViewModel() {
        InitFiles();

        SignPdfCommand = new Command<byte[]>(SignPdf);
        
        SharePdfCommand = new Command(SharePdf);
        OpenSignatureViewCommand = new Command(OpenSignatureView);
        CloseSignatureViewCommand = new Command(CloseSignatureView);
    }
    async void SharePdf() {
        await Share.Default.RequestAsync(new ShareFileRequest {
            Title = "Share the report",
            File = new ShareFile(DocumentFullPath)
        });
    }
    void OpenSignatureView() {
        IsSignatureViewOpened = true;
    }
    void CloseSignatureView() {
        IsSignatureViewOpened = false;
    }

    async void InitFiles() {
        certificateFullPath = await FileHelper.EnsureAssetInAppDataAsync(defaultCertificateName);
        
    }

    async void SignPdf(byte[] signatureImage) {
        CloseSignatureView();
        string signedPdfFullName = Path.Combine(FileSystem.Current.AppDataDirectory, Path.GetFileNameWithoutExtension(documentFullPath) + "_Signed1.pdf");
        IEnumerable<PdfFormFieldFacade> fields = GetDocumentFields();
        using var signer = new PdfDocumentSigner(documentFullPath);
        string signatureFieldName = null;
        var signatureField = fields.FirstOrDefault(f => f.Type == PdfFormFieldType.Signature) as PdfSignatureFormFieldFacade;
        if (signatureField == null)
            await Shell.Current.DisplayAlert("No Signature Fields Found", "A new signature field with a default position will be created", "OK");
        else {
            signatureFieldName = signatureField.FullName;
            signer.ClearSignatureField(signatureFieldName);
        }
        signer.SaveDocument(signedPdfFullName, CreateUserSignature(signatureFieldName, defaultCertificatePassword, "USA", "Jane Cooper", "Acknowledgement", signatureImage));
        documentFullPath = signedPdfFullName;
        UpdatePreview();
    }
    IEnumerable<PdfFormFieldFacade> GetDocumentFields() {
        using var processor = new PdfDocumentProcessor();
        processor.LoadDocument(documentFullPath);
        PdfDocumentFacade documentFacade = processor.DocumentFacade;
        PdfAcroFormFacade acroForm = documentFacade.AcroForm;
        return acroForm.GetFields();
    }
    PdfSignatureBuilder CreateUserSignature(string signatureFieldName, string password, string location, string contactInfo, string reason, byte[] signatureImage) {
        Pkcs7Signer pkcs7Signature = new Pkcs7Signer(certificateFullPath, password, HashAlgorithmType.SHA256);
        PdfSignatureBuilder userSignature;
        if (signatureFieldName == null)
            userSignature = new PdfSignatureBuilder(pkcs7Signature, new PdfSignatureFieldInfo(1) { SignatureBounds = new PdfRectangle(394, 254, 482, 286) });
        else
            userSignature = new PdfSignatureBuilder(pkcs7Signature, signatureFieldName);
        userSignature.Location = location;
        userSignature.Name = contactInfo;
        userSignature.Reason = reason;
        if (signatureImage != null) {
            userSignature.SetImageData(signatureImage);
        }
        return userSignature;
    }
    void UpdatePreview() {
        using Stream pdfStream = File.OpenRead(DocumentFullPath);
        var processor = new PdfDocumentProcessor() { RenderingEngine = PdfRenderingEngine.Skia };
        processor.LoadDocument(pdfStream);
        DXBitmap image = processor.CreateDXBitmap(1, 1200);

        using MemoryStream previewImageStream = new MemoryStream();
        image.Save(previewImageStream, DXImageFormat.Png);
        previewImageStream.Seek(0, SeekOrigin.Begin);
        var img = SKBitmap.Decode(previewImageStream);

        PdfPreview = (SKBitmapImageSource)img;
    }
    
    
    
    
    
    

    
    
    
    
    
    
    
    
    
    
    
    
    
    

    
}