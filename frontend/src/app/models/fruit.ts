export interface Fruit {
  id: number;
  name: string;
  description: string;
  price: number;
  discountPrice: number;
  stock: number;
  category: string;
  imageUrl: string;
  imageData?: string;
  isFeatured: boolean;
  status: string;
  priceUnit: string;
}
