<template>
  <v-menu v-model="menu" :close-on-content-click="false" offset-y left>
    <template #activator="{ on }">
      <v-btn icon v-on="on">
        <v-icon>mdi-cog</v-icon>
      </v-btn>
    </template>
    <v-card>
      <v-list>
        <v-subheader class="text-uppercase">
          {{ $t('Settings') }}
        </v-subheader>
        <v-list-item>
          <v-list-item-action>
            <v-switch
              :input-value="settings.sounds"
              :value="settings.sounds"
              color="primary"
              @change="saveSettings('sounds', $event)"
            />
          </v-list-item-action>
          <v-list-item-title>{{ $t('Sounds') }}</v-list-item-title>
        </v-list-item>
        <v-list-item v-if="gameFeedEnabled">
          <v-list-item-action>
            <v-switch
              :input-value="isMobile ? false : settings.gameFeed"
              :value="settings.gameFeed"
              :disabled="isMobile"
              color="primary"
              @change="saveSettings('gameFeed', $event)"
            />
          </v-list-item-action>
          <v-list-item-title>{{ $t('Game feed') }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-card>
  </v-menu>
</template>

<script>
import { mapState } from 'vuex'
import DeviceMixin from '~/mixins/Device'
import { config } from '~/plugins/config'

export default {
  mixins: [DeviceMixin],

  data () {
    return {
      menu: false
    }
  },

  computed: {
    ...mapState('settings', ['settings']),
    gameFeedEnabled () {
      return config('settings.interface.game_feed.enabled')
    }
  },

  methods: {
    saveSettings (key, value) {
      // value is null when switch is turned off
      this.$store.dispatch('settings/set', { key, value: !!value })
    }
  }
}
</script>
