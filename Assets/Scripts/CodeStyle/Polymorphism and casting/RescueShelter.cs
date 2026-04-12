namespace CodeStyle.PolymorphismAndCasting
{
    public class RescueShelter
    {
        public Mammal[] mammals;
        public RescueShelter()
        {
            mammals = new Mammal[2];
            mammals[0] = new Cat();
            mammals[1] = new Dog();

            if (mammals[0] is Cat)
            {
                Cat cat = mammals[0] as Cat;
                cat.Meow();
            }
            if (mammals[1] is Dog)
            {
                Dog dog = (Dog)mammals[1];
                dog.Woof();
            }
        }
    }
}