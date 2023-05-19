<template>
  <v-form ref="form" v-model="formIsValid">
    <v-row justify="center" no-gutters class="mt-5">
      <v-col cols="12" sm="6" md="4" lg="2">
        <v-text-field
          v-model.number="bet"
          dense
          outlined
          :full-width="false"
          class="text-center"
          :label="$t('Bet')"
          :disabled="inputDisabled"
          :rules="[validationInteger, v => validationMin(v, minBet), v => validationMax(v, Math.min(Math.floor(account.balance), maxBet))]"
          prepend-inner-icon="mdi-minus"
          append-icon="mdi-plus"
          @click:prepend-inner="decreaseBet"
          @click:append="increaseBet"
        >
          <template v-slot:prepend-inner>
            <v-btn small text icon color="primary" @click="bet = minBet">
              <v-icon small>
                mdi-arrow-down
              </v-icon>
            </v-btn>
            <v-btn small text icon color="primary" @click="decreaseBet">
              <v-icon small>
                mdi-minus
              </v-icon>
            </v-btn>
          </template>
          <template v-slot:append>
            <v-btn small text icon color="primary" @click="increaseBet">
              <v-icon small>
                mdi-plus
              </v-icon>
            </v-btn>
            <v-btn small text icon color="primary" @click="bet = maxBet">
              <v-icon small>
                mdi-arrow-up
              </v-icon>
            </v-btn>
          </template>
        </v-text-field>
      </v-col>
    </v-row>
    <v-row justify="center" no-gutters>
      <v-col class="text-center">
        <asset-search-modal
          :type="$route.params.packageId.replace('-prediction', '')"
          :default-asset-name="defaultAssetName"
          :disabled="searchDisabled"
          @change="handleAssetChange"
        >
          <template #default="{ on }">
            <v-btn
              outlined
              color="primary"
              :disabled="searchDisabled"
              class="mb-2 mb-sm-0"
              v-on="on"
            >
              {{ asset.name ? asset.name + ' ' + asset.symbol : $t('Loading...') }}
            </v-btn>
          </template>
        </asset-search-modal>
        <v-dialog
          v-model="durationModal"
          min-width="350"
          max-width="500"
          @click:outside="durationModal = false"
        >
          <template #activator="{ on }">
            <v-btn
              outlined
              color="primary"
              :disabled="inputDisabled || searchDisabled"
              class="mb-2 mb-sm-0"
              v-on="on"
            >
              {{ duration.text }}
            </v-btn>
          </template>
          <v-card>
            <v-toolbar>
              <v-toolbar-title>
                {{ $t('Choose duration') }}
              </v-toolbar-title>
              <v-spacer />
              <v-btn icon @click="durationModal = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar>
            <v-card-text>
              <v-select
                v-model="duration"
                :items="durations"
                :label="$t('Duration')"
                :disabled="inputDisabled"
                :return-object="true"
                hide-details
                outlined
                class="py-5"
                @change="durationModal = false"
              />
            </v-card-text>
          </v-card>
        </v-dialog>
      </v-col>
    </v-row>
    <v-row justify="center" no-gutters class="mt-5">
      <v-col cols="12" sm="6" md="4" lg="2" class="text-center text-no-wrap">
        <v-btn
          color="success"
          :loading="loading"
          :disabled="inputDisabled || !formIsValid || !balanceIsSufficient"
          @click="play(1)"
        >
          <v-icon left>
            mdi-trending-up
          </v-icon>
          {{ $t('Higher') }}
        </v-btn>
        <v-btn
          color="error"
          :loading="loading"
          :disabled="inputDisabled || !formIsValid || !balanceIsSufficient"
          @click="play(-1)"
        >
          <v-icon left>
            mdi-trending-down
          </v-icon>
          {{ $t('Lower') }}
        </v-btn>
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import { mapState } from 'vuex'
import FormMixin from '~/mixins/Form'
import PredictionMixin from '~/mixins/Prediction'
import SoundMixin from '~/mixins/Sound'
import clickSound from '~/../audio/common/click.wav'
import AssetSearchModal from '~/components/AssetSearchModal'

export default {
  components: { AssetSearchModal },

  mixins: [FormMixin, PredictionMixin, SoundMixin],

  props: {
    loading: {
      type: Boolean,
      required: true
    },
    inputDisabled: {
      type: Boolean,
      required: false,
      default: false
    },
    searchDisabled: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data () {
    return {
      durationModal: false,
      bet: null,
      duration: {},
      asset: {}
    }
  },

  computed: {
    ...mapState('auth', ['account']),
    defaultAssetName () {
      return this.config.default_asset_name
    },
    defaultBet () {
      return parseInt(this.config.default_bet_amount, 10)
    },
    minBet () {
      return parseInt(this.config.min_bet, 10)
    },
    maxBet () {
      return parseInt(this.config.max_bet, 10)
    },
    betStep () {
      return parseInt(this.config.bet_change_amount, 10)
    },
    balanceIsSufficient () {
      return this.account.balance >= this.bet
    },
    durations () {
      return this.config.durations
    }
  },

  created () {
    // it's important to wait until next tick to ensure config computed property is updated
    // after switching from one game page to another.
    this.$nextTick(() => {
      this.bet = this.defaultBet
      this.duration = this.durations[0]
    })
  },

  methods: {
    handleAssetChange (asset) {
      this.asset = { ...asset }
      this.$emit('asset', asset)
    },
    play (direction) {
      this.sound(clickSound)
      this.$emit('play', { asset: this.asset, bet: this.bet, direction, duration: this.duration.value })
    },
    decreaseBet () {
      this.sound(clickSound)
      const bet = this.bet - this.betStep
      this.bet = Math.max(this.minBet, bet)
    },
    increaseBet () {
      this.sound(clickSound)
      const bet = this.bet + this.betStep
      this.bet = Math.min(this.maxBet, bet)
    },
    setBet (value) {
      const minValues = this.autoPlay.maxBet > 0
        ? [value, this.maxBet, this.autoPlay.maxBet]
        : [value, this.maxBet]
      const maxValues = this.autoPlay.minBet > 0
        ? [this.minBet, this.autoPlay.minBet]
        : [this.minBet]
      this.bet = Math.max(...[Math.min(...minValues), ...maxValues])
    }
  }
}
</script>
<style lang="scss" scoped>
.scale-enter, .scale-leave-to {
  transform: scale(0);
  opacity: 1;
}

.scale-enter-active, .scale-leave-active {
  transition: all 0.3s;
}
</style>
