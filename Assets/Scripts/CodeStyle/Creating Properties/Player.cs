namespace CodeStyle.CreatingProperties
{
    public class Player
    {
        private int experience;

        public int Experience
        {
            get
            {
                //Some other code
                return experience;
            }
            set
            {
                //Some other code
                experience = value;
            }
        }

        public int Level
        {
            get
            {
                return experience / 1000;
            }
            set
            {
                experience = value * 1000;
            }
        }

        public int Health { get; set; }
    }
}
