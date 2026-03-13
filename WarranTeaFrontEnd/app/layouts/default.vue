<template>
  <v-app>
    <!-- App Bar -->
    <v-app-bar color="primary" density="comfortable" elevation="2">
      <v-app-bar-nav-icon v-if="isAuthenticated" @click="drawer = !drawer" />

      <v-app-bar-title
        class="d-flex align-center cursor-pointer"
        @click="navigateTo('/')"
      >
        <span class="text-secondary font-weight-bold text-h6">WarranTea</span>
        <v-icon class="ml-1" size="small">mdi-tea</v-icon>
      </v-app-bar-title>

      <v-spacer />

      <!-- Global Product Search -->
      <v-text-field
        v-if="isAuthenticated"
        v-model="globalSearch"
        class="mx-4 hidden-sm-and-down"
        density="compact"
        hide-details
        max-width="400"
        placeholder="Search for product warranty info..."
        prepend-inner-icon="mdi-magnify"
        rounded
        variant="solo-filled"
        @keyup.enter="handleGlobalSearch"
      />

      <template v-if="isAuthenticated">
        <v-btn icon @click="navigateTo('/dashboard')">
          <v-icon>mdi-view-dashboard</v-icon>
          <v-tooltip activator="parent" location="bottom">Dashboard</v-tooltip>
        </v-btn>

        <v-menu>
          <template #activator="{ props }">
            <v-btn icon v-bind="props">
              <v-avatar color="secondary" size="32">
                <span class="text-body-2 font-weight-bold">
                  {{ currentUser?.name?.charAt(0) ?? "U" }}
                </span>
              </v-avatar>
            </v-btn>
          </template>
          <v-list density="compact">
            <v-list-item
              prepend-icon="mdi-account"
              :title="currentUser?.name ?? 'User'"
            />
            <v-divider />
            <v-list-item
              prepend-icon="mdi-logout"
              title="Sign Out"
              @click="logout"
            />
          </v-list>
        </v-menu>
      </template>

      <template v-else>
        <v-btn color="secondary" variant="flat" @click="navigateTo('/login')">
          Sign In
        </v-btn>
        <v-btn
          color="secondary"
          variant="outlined"
          class="ml-2"
          @click="navigateTo('/register')"
        >
          Sign Up
        </v-btn>
      </template>
    </v-app-bar>

    <!-- Navigation Drawer -->
    <v-navigation-drawer
      v-if="isAuthenticated"
      v-model="drawer"
      :rail="rail"
      @click="rail = false"
    >
      <v-list density="compact" nav>
        <v-list-item
          prepend-icon="mdi-view-dashboard"
          title="Dashboard"
          to="/dashboard"
          value="dashboard"
        />
        <v-list-item
          prepend-icon="mdi-file-upload"
          title="Add Warranty"
          to="/warranties/add"
          value="add"
        />
        <v-list-item
          prepend-icon="mdi-magnify"
          title="Search Warranties"
          to="/warranties/search"
          value="search"
        />
        <v-list-item
          prepend-icon="mdi-database-search"
          title="Product Lookup"
          to="/products/search"
          value="products"
        />
      </v-list>

      <template #append>
        <v-list density="compact" nav>
          <v-list-item
            :prepend-icon="rail ? 'mdi-chevron-right' : 'mdi-chevron-left'"
            title="Collapse"
            @click.stop="rail = !rail"
          />
        </v-list>
      </template>
    </v-navigation-drawer>

    <!-- Main Content -->
    <v-main>
      <slot />
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
const { currentUser, isAuthenticated, logout } = useAuth();

const drawer = ref(true);
const rail = ref(false);
const globalSearch = ref("");

function handleGlobalSearch() {
  if (globalSearch.value.trim()) {
    navigateTo({ path: "/products/search", query: { q: globalSearch.value } });
    globalSearch.value = "";
  }
}
</script>
