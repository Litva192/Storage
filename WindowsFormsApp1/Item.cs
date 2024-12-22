namespace StorageApp {
    public class Item {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public Item(string name, int quantity) {
            Name = name;
            Quantity = quantity;
        }

        public override string ToString() {
            return $"Name: {Name}, Quantity: {Quantity}";
        }
    }
}
