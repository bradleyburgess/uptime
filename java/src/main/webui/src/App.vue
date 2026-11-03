<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'
import StatusBadge from './components/StatusBadge.vue'
import OutagesTable from './components/OutagesTable.vue'
import { Configuration, DefaultApi, type UptimeDto } from './api'
import { formatDate } from './utils/stringUtils.ts'
const api = new DefaultApi(new Configuration({ basePath: '' }))

const loading = ref(true)
const uptime = ref<UptimeDto | null>(null)
const outages = computed(() => uptime.value?.outages ?? [])
const currentStatus = computed(() => uptime.value?.currentStatus ?? null)
const version = ref<string | null>(null)

async function fetchUptime() {
  const response = await api.apiCurrentStatusGet()
  uptime.value = response ?? null
}

async function fetchVersion() {
  const response = await api.apiVersionGet()
  version.value = response.version ?? null
}

let interval: number | null = null

onMounted(async () => {
  await fetchUptime()
  await fetchVersion()
  loading.value = false
  interval = setInterval(fetchUptime, 5000)
})

onUnmounted(() => {
  if (interval !== null) {
    clearInterval(interval)
  }
})
</script>

<template v-if="!loading">
  <div class="min-h-screen flex flex-col">
    <main class="flex-1 w-full max-w-5xl mx-auto p-2 py-4 space-y-12">
      <header>
        <h1 class="text-4xl font-semibold text-center sm:text-start">Uptime</h1>
      </header>
      <section class="p-4 rounded-2xl bg-stone-900 flex flex-col space-y-3">
        <h2 class="text-xl font-semibold">Current Status</h2>
        <p>
          Monitoring
          <code class="mx-2 px-2 rounded font-mono inline-block bg-stone-800 text-amber-500">{{
            currentStatus?.monitoringHost
          }}</code>
        </p>
        <div class="flex gap-4 items-center">
          <StatusBadge :status="currentStatus?.status" />
          <p>Last successful ping: {{ formatDate(currentStatus?.lastSuccessfulPing) }}</p>
        </div>
      </section>
      <section class="p-4 rounded-2xl bg-stone-900 flex flex-col space-y-3">
        <h2 class="text-xl font-semibold">Recent Outages</h2>
        <p v-if="outages == null || outages.length === 0" class="text-stone-400">
          No recent outages reported
        </p>
        <OutagesTable v-else :tableData="outages" />
      </section>
    </main>
    <footer class="w-full max-w-5xl mx-auto p-2 md:py-4 text-sm text-stone-400 text-center">
      <p>
        Uptime<span v-if="version"> v{{ version }}</span>
      </p>
      <p>Copyright © 2024. Bradley Burgess.</p>
    </footer>
  </div>
</template>
