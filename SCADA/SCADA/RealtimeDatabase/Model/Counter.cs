
namespace SCADA.RealtimeDatabase.Model
{
    public class Counter : ProcessVariable
    {
        public Counter()
        {
            this.Type = VariableTypes.COUNTER;
        }

        public int Value { get; set; }
    }
}
