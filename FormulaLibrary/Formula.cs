namespace FormulaLibrary
{
    public abstract class Formula
    {
        public abstract FormulaType getType();
        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}