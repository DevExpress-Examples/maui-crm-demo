using System.Windows.Input;
using DevExpress.Maui.Mvvm;
using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;
using CrmDemo.Helpers;
using DevExpress.Maui.Pdf;

namespace CrmDemo.ViewModels.Orders;

[QueryProperty(nameof(DocumentFullPath), "documentFullPath")]
public class OrderPdfPreviewViewModel : DXObservableObject {
    #region fields
    PdfDocumentSource pdfPreview;
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
            OnPropertyChanged(nameof(DocumentFullPath));
            UpdatePreview();
        }
    }
    public PdfDocumentSource PdfPreview {
        get {
            return pdfPreview;
        }
        set {
            pdfPreview = value;
            OnPropertyChanged(nameof(PdfPreview));
        }
    }
    public bool IsSignatureViewOpened {
        get {
            return isSignatureViewOpened;
        }
        set {
            isSignatureViewOpened = value;
            OnPropertyChanged(nameof(IsSignatureViewOpened));
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
        //OpenFileCommand = new Command(OpenFile);
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
        //documentFullPath = await CopyWorkingFilesToAppData(defaultDocumentName);
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
        PdfPreview = PdfDocumentSource.FromFile(DocumentFullPath);
    }
    //private async void OpenFile() {
    //    await PickAndShow(new PickOptions {
    //        PickerTitle = "Select a PDF file",
    //        FileTypes = FilePickerFileType.Pdf
    //    });
    //}

    //public async Task PickAndShow(PickOptions options) {
    //    try {
    //        var result = await FilePicker.Default.PickAsync(options);
    //        if (result != null) {
    //            if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase)) {
    //                var stream = await result.OpenReadAsync();
    //                documentFullPath = result.FullPath;
    //                UpdatePreview();
    //            }
    //        }
    //    }
    //    catch {
    //        // The user canceled or something went wrong
    //    }

    //}
}