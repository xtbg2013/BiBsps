namespace BiBsps.BiOven
{
    public abstract class BaseOvenControl
    {
        public abstract double GetBoardTemperature(int floor, int number);
        public abstract void SetBoardTemperature(int floor, int number, double temperature);
        public abstract void StartBoardOven(int floor, int number);
        public abstract void StopBoardOven(int floor, int number);
        public abstract void SetTempOffset(int floor, int number, double temperature);
    }
}
