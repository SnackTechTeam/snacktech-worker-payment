namespace WorkerService.Handlers
{
    public interface IMensagemHandler
    {
        public Task Processar(Amazon.SQS.Model.Message mensagem);
    }
}