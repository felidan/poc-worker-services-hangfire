using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace PocJobsHangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        [HttpGet]
        public string Criar()
        {
            var inicio = $"Request: {DateTime.Now}";

            // Executa uma única vez quando criado
            var jobFireForget = BackgroundJob.Enqueue(() => JobFireForget());

            // Executa uma vez após um determinado tempo
            var jobDelayed = BackgroundJob.Schedule(() => JobSchedule(), TimeSpan.FromSeconds(30));

            // Executa após um job ser executado
            BackgroundJob.ContinueWith(jobDelayed, () => JobBackground());

            // Executa recorrente com um expressão Cron
            RecurringJob.AddOrUpdate(() => JobRecurring(), Cron.Minutely);

            return inicio + " - Jobs criados com sucesso!";
        }

        [DisplayName("Job - Fire and forget Teste")]
        public void JobFireForget()
        {
            Debug.WriteLine($"Fire and forget: {DateTime.Now}");
        }

        [DisplayName("Job - Schedule Delayed Teste")]
        public void JobSchedule()
        {
            Debug.WriteLine($"Delayed: {DateTime.Now}");
        }

        [DisplayName("Job - Background Teste")]
        public void JobBackground()
        {
            Debug.WriteLine($"Continuation: {DateTime.Now}");
        }

        [DisplayName("Job - Gravar log")]
        public void JobRecurring()
        {
            Log();
            Debug.WriteLine($"Recurring: {DateTime.Now}");
        }

        private void Log()
        {
            string path = @"D:\log.txt";
            string log = "";
            string newLog = $"Gerando novo log em {DateTime.Now.ToLongTimeString()}";

            using(StreamReader stream = new StreamReader(path))
            {
                log = stream.ReadToEnd();
            }

            using(StreamWriter stream = new StreamWriter(path))
            {
                stream.WriteLine(log + newLog + Environment.NewLine);
            }
        }
    }
}