using System;
using System.Threading.Tasks;

namespace AsyncLearning
{
    public class FormActions
    {
        public void AssignForm(Form1 form)
        {
            this.form = form;   
        }

        public async void OnClick()
        {
            Log("Action!");
            Task task = DoSomeWorkAsync();
            Log("After DoSomeWorkAsync!");
        }

        private async Task DoSomeWorkAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            Log("Work done!");
        }

        private void Log(string logText)
        {
            form.AddLog(logText);
        }

        private Form1 form;
    }
}