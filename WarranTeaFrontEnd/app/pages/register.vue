<template>
  <v-container class="py-16" max-width="480">
    <v-card class="pa-8" elevation="4" rounded="xl">
      <div class="text-center mb-6">
        <v-icon color="primary" size="48">mdi-tea</v-icon>
        <h1 class="text-h4 font-weight-bold mt-2">Create Account</h1>
        <p class="text-body-2 text-grey">
          Join WarranTea and keep your warranties safe
        </p>
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

      <v-form
        ref="formRef"
        v-model="formValid"
        @submit.prevent="handleRegister"
      >
        <v-text-field
          v-model="name"
          class="mb-3"
          label="Full Name"
          prepend-inner-icon="mdi-account"
          :rules="[rules.required]"
          variant="outlined"
        />

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
          :rules="[rules.required, rules.minLength]"
          :type="showPassword ? 'text' : 'password'"
          variant="outlined"
          :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
          @click:append-inner="showPassword = !showPassword"
        />

        <v-text-field
          v-model="confirmPassword"
          class="mb-3"
          label="Confirm Password"
          prepend-inner-icon="mdi-lock-check"
          :rules="[rules.required, rules.match]"
          :type="showPassword ? 'text' : 'password'"
          variant="outlined"
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
          Create Account
        </v-btn>
      </v-form>

      <div class="text-center mt-6">
        <span class="text-body-2 text-grey">Already have an account?</span>
        <v-btn
          color="primary"
          variant="text"
          size="small"
          @click="navigateTo('/login')"
        >
          Sign In
        </v-btn>
      </div>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
definePageMeta({ layout: "default" });

const { register } = useAuth();

const formRef = ref();
const formValid = ref(false);
const loading = ref(false);
const error = ref("");
const name = ref("");
const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const showPassword = ref(false);

const rules = {
  required: (v: string) => !!v || "This field is required",
  email: (v: string) => /.+@.+\..+/.test(v) || "Enter a valid email",
  minLength: (v: string) =>
    v.length >= 6 || "Password must be at least 6 characters",
  match: (v: string) => v === password.value || "Passwords do not match",
};

async function handleRegister() {
  if (!formValid.value) return;
  loading.value = true;
  error.value = "";
  try {
    await register(name.value, email.value, password.value);
    navigateTo("/dashboard");
  } catch (e: any) {
    const errors = e?.data?.errors;
    if (Array.isArray(errors)) {
      error.value = errors.join(" ");
    } else {
      error.value =
        e?.data?.message ||
        e?.message ||
        "Registration failed. Please try again.";
    }
  } finally {
    loading.value = false;
  }
}
</script>
