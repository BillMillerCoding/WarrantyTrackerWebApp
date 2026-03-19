import type { Product, SearchResult } from "~/types";

interface ApiSearchResult {
  productWarrantyId: number;
  productId: number;
  productName: string;
  brand: string;
  category: string;
  coverageType: string;
  durationMonths: number;
  provider: string;
  description?: string;
  url?: string;
  imageUrl?: string;
  score?: number;
}

interface ApiSearchResponse {
  results: ApiSearchResult[];
  totalCount: number;
}

function formatDuration(months: number): string {
  if (months >= 12 && months % 12 === 0) {
    const years = months / 12;
    return years === 1 ? "1 Year" : `${years} Years`;
  }
  return `${months} Months`;
}

function mapResult(r: ApiSearchResult): Product {
  return {
    id: String(r.productWarrantyId),
    name: r.productName,
    brand: r.brand,
    category: r.category,
    warrantyDuration: formatDuration(r.durationMonths),
    warrantyType: r.coverageType,
    description: r.description,
    imageUrl: r.imageUrl,
    score: r.score,
  };
}

export async function searchProducts(query: string): Promise<SearchResult> {
  const response = await $fetch<ApiSearchResponse>("/api/products/search", {
    params: { q: query },
  });
  return {
    products: response.results.map(mapResult),
    total: response.totalCount,
  };
}

export async function getProductById(id: string): Promise<Product | undefined> {
  try {
    const r = await $fetch<ApiSearchResult>(`/api/products/${id}`);
    return mapResult(r);
  } catch {
    return undefined;
  }
}
