import type { Product } from "../../../types/product";
import type { RecommendationProduct } from "../../../types/recommendation";

export function toRecommendationProduct(product: Product): RecommendationProduct {
  return {
    id: product.id,
    name: product.name,
    description: product.description,
    price: product.price,
    category: product.category ?? inferRecommendationCategory(product.name),
  };
}

export function getFallbackProducts(products: Product[], currentProductId: string) {
  return products
    .filter((product) => product.id !== currentProductId)
    .slice(0, 2);
}

function inferRecommendationCategory(name: string) {
  const normalized = name.toLowerCase();

  if (normalized.includes("keyboard")) return "Keyboards";
  if (normalized.includes("mouse")) return "Accessories";
  if (normalized.includes("monitor") || normalized.includes("display")) {
    return "Monitors";
  }
  if (
    normalized.includes("processor") ||
    normalized.includes("cpu") ||
    normalized.includes("ryzen")
  ) {
    return "Processors";
  }
  if (
    normalized.includes("storage") ||
    normalized.includes("ssd") ||
    normalized.includes("drive")
  ) {
    return "Storage";
  }

  return "Accessories";
}
