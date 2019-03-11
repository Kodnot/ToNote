namespace ToNote.Logic.Dialog
{
    using ToNote.Interfaces;
    using ToNote.Views;
    using ToNote.ViewModels;

    public class DialogService
    {
        public static T OpenDialog<T>(BaseDialogViewModel<T> viewModel)
        {
            IDialogWindow window = new BaseDialogWindow();
            window.DataContext = viewModel;
            window.ShowDialog();
            return viewModel.DialogResult;
        }
    }
}
