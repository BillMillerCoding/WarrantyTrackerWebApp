export default defineNuxtRouteMiddleware(async (to) => {
  if (import.meta.server) return;

  const { isAuthenticated, initialized, fetchUser } = useAuth();

  if (!initialized.value) {
    await fetchUser();
  }

  const publicPages = ["/", "/login", "/register", "/products/search"];
  if (!isAuthenticated.value && !publicPages.includes(to.path)) {
    return navigateTo("/login");
  }
});
