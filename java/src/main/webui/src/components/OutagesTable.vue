<script setup lang="ts">
import { formatDate } from '@/utils/stringUtils'
import type { Outage } from '@/api'

import { defineProps } from 'vue'
import { calculateDuration } from '@/utils/dateUtils'

const { tableData } = defineProps<{
  tableData: Outage[]
}>()
</script>

<template>
  <div class="w-full overflow-hidden">
    <div class="overflow-x-auto">
      <table class="w-full text-left text-sm text-stone-300">
        <thead class="border-b border-stone-700 text-sm font-semibold tracking-wider text-stone-100">
          <tr>
            <th scope="col" class="px-4 py-3">Start Time</th>
            <th scope="col" class="px-4 py-3">End Time</th>
            <th scope="col" class="px-4 py-3">Duration</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-stone-800">
          <tr
            v-for="(row, index) in tableData"
            :key="index"
            class="transition-colors hover:bg-stone-800/50"
          >
            <td class="whitespace-nowrap px-4 py-3 font-medium">
              {{ formatDate(row.startDate) }}
            </td>
            <td class="whitespace-nowrap px-4 py-3">
              {{ formatDate(row.endDate) }}
            </td>
            <td class="whitespace-nowrap px-4 py-3">
              <span
                class="inline-flex items-center rounded-md bg-amber-500/10 px-2.5 py-1 text-xs font-medium text-amber-500 ring-1 ring-inset ring-amber-500/30"
              >
                {{ calculateDuration(row.startDate!, row.endDate) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
