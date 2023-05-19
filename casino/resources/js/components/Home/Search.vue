<template>
  <v-container class="home-page-search mt-10">
    <v-row>
      <v-col md="6" class="d-flex align-center justify-end">
        <h2 class="font-weight-thin">
          {{ $t('Let\'s play?') }}
        </h2>
      </v-col>
      <v-col md="6">
        <game-search-modal>
          <template #default="{ on }">
            <v-btn
              rounded
              outlined
              large
              v-on="on"
            >
              {{ $t('Choose a game') }}
            </v-btn>
          </template>
        </game-search-modal>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import GameSearchModal from '~/components/GameSearchModal'

export default {
  components: { GameSearchModal }
}
</script>
