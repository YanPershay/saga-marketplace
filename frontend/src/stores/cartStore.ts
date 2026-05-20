import { create } from "zustand";
import type { Product } from "../types/product";

export type CartItem = {
  productId: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  category?: string;
  imageUrl?: string;
};

type CartStore = {
  items: CartItem[];
  addProduct: (product: Product) => void;
  removeItem: (productId: string) => void;
  increaseQuantity: (productId: string) => void;
  decreaseQuantity: (productId: string) => void;
  clearCart: () => void;
  getTotalItems: () => number;
  getTotalPrice: () => number;
};

export const useCartStore = create<CartStore>((set, get) => ({
  items: [],
  addProduct: (product) => {
    set((state) => {
      const existingItem = state.items.find((item) => item.productId === product.id);

      if (existingItem) {
        return {
          items: state.items.map((item) =>
            item.productId === product.id
              ? { ...item, quantity: item.quantity + 1 }
              : item,
          ),
        };
      }

      return {
        items: [
          ...state.items,
          {
            productId: product.id,
            name: product.name,
            description: product.description,
            price: product.price,
            quantity: 1,
            category: product.category,
            imageUrl: product.imageUrl,
          },
        ],
      };
    });
  },
  removeItem: (productId) => {
    set((state) => ({
      items: state.items.filter((item) => item.productId !== productId),
    }));
  },
  increaseQuantity: (productId) => {
    set((state) => ({
      items: state.items.map((item) =>
        item.productId === productId
          ? { ...item, quantity: item.quantity + 1 }
          : item,
      ),
    }));
  },
  decreaseQuantity: (productId) => {
    set((state) => ({
      items: state.items
        .map((item) =>
          item.productId === productId
            ? { ...item, quantity: item.quantity - 1 }
            : item,
        )
        .filter((item) => item.quantity > 0),
    }));
  },
  clearCart: () => {
    set({ items: [] });
  },
  getTotalItems: () =>
    get().items.reduce((total, item) => total + item.quantity, 0),
  getTotalPrice: () =>
    get().items.reduce(
      (total, item) => total + item.price * item.quantity,
      0,
    ),
}));
