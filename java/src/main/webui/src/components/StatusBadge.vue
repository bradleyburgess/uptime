<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { SystemStatus } from '../api'
const { status } = defineProps<{
  status: SystemStatus | null | undefined
}>()

const text = computed(() => (status === SystemStatus.Offline ? 'Offline' : 'Online'))
const badgeClasses = computed(() => {
  return {
    'text-stone-50 text-sm font-medium rounded-full px-4 py-1.5': true,
    'bg-red-600': status === SystemStatus.Offline,
    'bg-green-600': status === SystemStatus.Online,
  }
})

onMounted(() => {
  console.log(`From StatusBadge: ${status}`)
})
</script>

<template>
  <div :class="badgeClasses">
    {{ text }}
  </div>
</template>
