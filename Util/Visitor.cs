namespace RubbishLanguageFrontEnd.Util {

    public interface IVisitable {
        public void Accept(IVisitor visitor);
    }

    public interface IVisitor {
        public void Visit(IVisitable visitable);
    }
}
