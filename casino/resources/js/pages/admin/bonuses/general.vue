<template>
  <v-container>
    <data-table
      api="/api/admin/bonuses"
      :title="$t('Bonuses')"
      :headers="headers"
      :filters="['period', 'user-role']"
      :search="true"
      :search-placeholder="$t('User name or email')"
    >
      <template v-slot:item.name="{ item }">
        <user-link :user="item.account.user" />
      </template>
    </data-table>
  </v-container>
</template>

<script>
import DataTable from '~/components/DataTable'
import UserLink from '~/components/Admin/UserLink'

export default {
  components: { DataTable, UserLink },

  middleware: ['auth', 'verified', '2fa_passed', 'admin'],

  metaInfo () {
    return { title: this.$t('Bonuses') }
  },

  computed: {
    headers () {
      return [
        { text: this.$t('ID'), value: 'id' },
        { text: this.$t('Name'), value: 'name', sortable: false },
        { text: this.$t('Type'), value: 'title', sortable: false },
        { text: this.$t('Amount'), value: 'amount', align: 'right', format: 'decimal' },
        { text: this.$t('Created'), value: 'created_ago', align: 'right' }
      ]
    }
  }
}
</script>
