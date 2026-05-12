namespace Smarthome
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var login = new FrmLogin();
            if (login.ShowDialog() == DialogResult.OK)
            {
                var main = new FormMain();
                main.CurrentUsername = login.LoggedInUsername;
                main.CurrentRole = login.LoggedInRole;
                main.CurrentAccountId = login.LoggedInAccountId;
                Application.Run(main);
            }
        }
    }
}