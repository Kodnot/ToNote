namespace ToNote.ViewModels
{
    using ToNote.Interfaces;
    using ToNote.Models;

    public abstract class BaseDialogViewModel<T> : BaseModel
    {
        public T DialogResult { get; set; }

        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            DialogResult = result;

            if (dialog != null)
                dialog.DialogResult = true;
        }
    }
}
