namespace BiBsps.BiGlobalFiies
{
    public abstract class BaseCom
    {
        public abstract byte[] Query(byte[] cmd, int sleep = 5000, int waitTranferDone = 500);
    }
}
