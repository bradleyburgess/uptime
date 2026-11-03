import { format } from 'date-fns'

export const formatDate = (date?: Date) =>
  date ? format(date, 'd MMMM yyyy, HH:mm:ss') : 'Unknown'
