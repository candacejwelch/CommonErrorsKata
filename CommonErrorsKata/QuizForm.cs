using CommonErrorsKata.Shared;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace CommonErrorsKata
{
    public partial class CommonErrorsForm : Form
    {
        private readonly AnswerQueue<TrueFalseAnswer> answerQueue;
        private readonly string[] files;
        private readonly SynchronizationContext synchronizationContext;
        private int percentageTimer = 100;
        private string currentBaseName = null;
        private readonly string[] possibleAnswers = null;

        public CommonErrorsForm()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            files = Directory.GetFiles(Environment.CurrentDirectory +  @"..\..\..\ErrorPics");
            possibleAnswers = new string[] { "missing file", "null instance", "divide by zero" };
            lstAnswers.DataSource = possibleAnswers;
            answerQueue = new AnswerQueue<TrueFalseAnswer>(15);
            Next();
            lstAnswers.Click += LstAnswers_Click;
            StartTimer();
        }
        private async void StartTimer()
        {
            await Task.Run(() =>
            {
                for (percentageTimer = 100; percentageTimer > 0; percentageTimer--)
                {
                    UpdateProgress(percentageTimer);
                    Thread.Sleep(50);
                }
                Message("Need to be quicker on your feet next time!  Try again...");
            });
        }

        private void LstAnswers_Click(object sender, EventArgs e)
        {
            percentageTimer = 100;
            //var tokens = currentBaseName.Split(' ');
            //TODO:  Figure out what is a valid answer.

            foreach (var answer in possibleAnswers)
            {
                if (currentBaseName == answer)
                {
                    Console.WriteLine(answer);
                    answerQueue.Enqueue(new TrueFalseAnswer(true));
                    
                }
                else answerQueue.Enqueue(new TrueFalseAnswer(false));
            }

            //answerQueue.Enqueue(new TrueFalseAnswer(true));
            Next();
        }

        private void Next()
        {
            if (answerQueue.Count == 15 && answerQueue.Grade >= 98)
            {
                MessageBox.Show("Congratulations you've defeated me!");
                Application.Exit();
                return;
            }
            label1.Text = answerQueue.Grade.ToString() + "%";
            var file = files.GetRandom();
            currentBaseName= Path.GetFileNameWithoutExtension(file);
            pbImage.ImageLocation = file;
        }

        public void UpdateProgress(int value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x => {
                progress.Value = value;
            }), value);
        }
        public void Message(string value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x => {
                MessageBox.Show(value);
            }), value);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
