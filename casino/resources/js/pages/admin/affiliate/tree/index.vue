<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-toolbar>
            <v-toolbar-title>
              {{ $t('Affiliates tree') }}
            </v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-treeview v-if="tree" :items="tree">
              <template #label="{ item }">
                {{ item.name }}{{ item.children && item.children.length ? ` (${item.children.length})` : '' }}
              </template>
              <template #append="{ item }">
                <affiliate-tree-menu :id="item.id" small />
              </template>
            </v-treeview>
            <template v-else>
              <p>
                {{ $t('Loading...') }}
              </p>
            </template>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from 'axios'
import AffiliateTreeMenu from '~/components/Admin/AffiliateTreeMenu'

export default {
  components: { AffiliateTreeMenu },
  middleware: ['auth', 'verified', '2fa_passed', 'admin'],

  metaInfo () {
    return { title: this.$t('Affiliates tree') }
  },

  data () {
    return {
      tree: null
    }
  },

  async created () {
    const { data } = await axios.get('/api/admin/affiliate/tree')

    this.tree = data
  }
}
</script>
