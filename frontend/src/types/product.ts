export type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  quantityAvailable: number;
  createdAt: string;
  category?: string;
  imageUrl?: string;
};
