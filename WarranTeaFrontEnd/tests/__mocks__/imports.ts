// Mock Nuxt auto-imports for tests
import { ref, computed, reactive } from "vue";

export { ref, computed, reactive };

export function useState<T>(key: string, init: () => T) {
  const state = ref(init()) as any;
  return state;
}

export function navigateTo(path: string) {
  return path;
}

export function definePageMeta(_meta: any) {}

export function useRuntimeConfig() {
  return { public: {} };
}

export async function $fetch<T = any>(url: string, opts?: any): Promise<T> {
  throw new Error(`$fetch not mocked for: ${url}`);
}
