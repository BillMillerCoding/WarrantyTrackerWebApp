import type { User } from "~/types";
import { useApiBase } from "~/composables/useApi";

export function useAuth() {
  const apiBase = useApiBase();
  const currentUser = useState<User | null>("auth-user", () => null);
  const isAuthenticated = computed(() => currentUser.value !== null);
  const initialized = useState("auth-initialized", () => false);

  async function fetchUser() {
    try {
      const user = await $fetch<User & { roles?: string[] }>(
        `${apiBase}/auth/me`,
        { credentials: "include" },
      );
      currentUser.value = user;
    } catch {
      currentUser.value = null;
    }
    initialized.value = true;
  }

  async function login(email: string, password: string) {
    const resp = await $fetch<User & { roles?: string[] }>(
      `${apiBase}/auth/login`,
      {
        method: "POST",
        body: { email, password },
        credentials: "include",
      },
    );
    currentUser.value = {
      id: resp.id,
      name: resp.name,
      email: resp.email,
      roles: resp.roles,
    };
    initialized.value = true;
  }

  async function register(name: string, email: string, password: string) {
    const resp = await $fetch<User & { roles?: string[] }>(
      `${apiBase}/auth/register`,
      {
        method: "POST",
        body: { name, email, password },
        credentials: "include",
      },
    );
    currentUser.value = {
      id: resp.id,
      name: resp.name,
      email: resp.email,
      roles: resp.roles,
    };
    initialized.value = true;
  }

  async function logout() {
    try {
      await $fetch(`${apiBase}/auth/logout`, {
        method: "POST",
        credentials: "include",
      });
    } catch {
      // ignore errors on logout
    }
    currentUser.value = null;
    navigateTo("/");
  }

  const isAdmin = computed(
    () => currentUser.value?.roles?.includes("Admin") ?? false,
  );

  return {
    currentUser,
    isAuthenticated,
    isAdmin,
    initialized,
    fetchUser,
    login,
    register,
    logout,
  };
}
