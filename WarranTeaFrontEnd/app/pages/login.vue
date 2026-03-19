<template>
  <v-container class="py-16" max-width="480">
    <v-card class="pa-8" elevation="4" rounded="xl">
      <div class="text-center mb-6">
        <v-icon color="primary" size="48">mdi-tea</v-icon>
        <h1 class="text-h4 font-weight-bold mt-2">Welcome Back</h1>
        <p class="text-body-2 text-grey">Sign in to your WarranTea account</p>
      </div>

      <v-alert
        v-if="error"
        type="error"
        variant="tonal"
        class="mb-4"
        closable
        @click:close="error = ''"
      >
        {{ error }}
      </v-alert>

      <v-form ref="formRef" v-model="formValid" @submit.prevent="handleLogin">
        <v-text-field
          v-model="email"
          class="mb-3"
          label="Email"
          prepend-inner-icon="mdi-email"
          :rules="[rules.required, rules.email]"
          type="email"
          variant="outlined"
        />

        <v-text-field
          v-model="password"
          class="mb-3"
          label="Password"
          prepend-inner-icon="mdi-lock"
          :rules="[rules.required]"
          :type="showPassword ? 'text' : 'password'"
          variant="outlined"
          :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
          @click:append-inner="showPassword = !showPassword"
        />

        <v-btn
          block
          color="primary"
          :disabled="!formValid"
          :loading="loading"
          size="large"
          type="submit"
          variant="flat"
        >
          Sign In
        </v-btn>
      </v-form>

      <div class="text-center mt-6">
        <span class="text-body-2 text-grey">Don't have an account?</span>
        <v-btn
          color="primary"
          variant="text"
          size="small"
          @click="navigateTo('/register')"
        >
          Sign Up
        </v-btn>
      </div>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
definePageMeta({ layout: "default" });

const { login } = useAuth();

const formRef = ref();
const formValid = ref(false);
const loading = ref(false);
const error = ref("");
const email = ref("");
const password = ref("");
const showPassword = ref(false);

const rules = {
  required: (v: string) => !!v || "This field is required",
  email: (v: string) => /.+@.+\..+/.test(v) || "Enter a valid email",
};

async function handleLogin() {
  if (!formValid.value) return;
  loading.value = true;
  error.value = "";
  try {
    await login(email.value, password.value);
    navigateTo("/dashboard");
  } catch (e: any) {
    error.value =
      e?.data?.message || e?.message || "Login failed. Please try again.";
  } finally {
    loading.value = false;
  }
}
</script>
