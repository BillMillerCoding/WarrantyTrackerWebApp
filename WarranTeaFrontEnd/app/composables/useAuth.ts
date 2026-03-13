import type { User } from "~/types";

export function useAuth() {
  const currentUser = useState<User | null>("auth-user", () => null);
  const isAuthenticated = computed(() => currentUser.value !== null);
  const initialized = useState("auth-initialized", () => false);

  async function fetchUser() {
    try {
      const user = await $fetch<User>("/api/auth/me");
      currentUser.value = user;
    } catch {
      currentUser.value = null;
    }
    initialized.value = true;
  }

  async function login(email: string, password: string) {
    const user = await $fetch<User>("/api/auth/login", {
      method: "POST",
      body: { email, password },
    });
    currentUser.value = user;
    initialized.value = true;
  }

  async function register(name: string, email: string, password: string) {
    const user = await $fetch<User>("/api/auth/register", {
      method: "POST",
      body: { name, email, password },
    });
    currentUser.value = user;
    initialized.value = true;
  }

  async function logout() {
    try {
      await $fetch("/api/auth/logout", { method: "POST" });
    } catch {
      // ignore errors on logout
    }
    currentUser.value = null;
    navigateTo("/");
  }

  return {
    currentUser,
    isAuthenticated,
    initialized,
    fetchUser,
    login,
    register,
    logout,
  };
}
