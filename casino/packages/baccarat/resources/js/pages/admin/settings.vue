<template>
  <v-card flat>
    <v-card-text>
      <v-expansion-panels>
        <v-expansion-panel>
          <v-expansion-panel-header>{{ $t('General') }}</v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-combobox
              v-model="form.GAME_BACCARAT_CATEGORIES"
              :label="$t('Categories')"
              multiple
              outlined
              chips
              small-chips
              deletable-chips
              hide-selected
              no-filter
            />

            <file-upload
              v-model="form.GAME_BACCARAT_BANNER"
              :label="$t('Banner')"
              name="banner"
              :folder="`games/${packageId}`"
            />

            <v-text-field
              v-model.number="form.GAME_BACCARAT_MIN_BET"
              :label="$t('Min bet')"
              :rules="[validationInteger, validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_MIN_BET')"
              :error-messages="form.errors.get('GAME_BACCARAT_MIN_BET')"
              outlined
              :suffix="$t('credits')"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_MIN_BET')"
            />

            <v-text-field
              v-model.number="form.GAME_BACCARAT_MAX_BET"
              :label="$t('Max bet')"
              :rules="[validationInteger, validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_MAX_BET')"
              :error-messages="form.errors.get('GAME_BACCARAT_MAX_BET')"
              outlined
              :suffix="$t('credits')"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_MAX_BET')"
            />

            <v-text-field
              v-model.number="form.GAME_BACCARAT_BET_CHANGE_AMOUNT"
              :label="$t('Bet increment / decrement amount')"
              :rules="[validationInteger, validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_BET_CHANGE_AMOUNT')"
              :error-messages="form.errors.get('GAME_BACCARAT_BET_CHANGE_AMOUNT')"
              outlined
              :suffix="$t('credits')"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_BET_CHANGE_AMOUNT')"
            />

            <v-text-field
              v-model.number="form.GAME_BACCARAT_DEFAULT_BET_AMOUNT"
              :label="$t('Default bet amount')"
              :rules="[validationInteger, validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_DEFAULT_BET_AMOUNT')"
              :error-messages="form.errors.get('GAME_BACCARAT_DEFAULT_BET_AMOUNT')"
              outlined
              :suffix="$t('credits')"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_DEFAULT_BET_AMOUNT')"
            />
          </v-expansion-panel-content>
        </v-expansion-panel>
        <v-expansion-panel>
          <v-expansion-panel-header>{{ $t('Paytable') }}</v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-text-field
              v-model.number="form.GAME_BACCARAT_PAYOUT_PLAYER"
              :label="$t('Player bet')"
              :rules="[validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_PAYOUT_PLAYER')"
              :error-messages="form.errors.get('GAME_BACCARAT_PAYOUT_PLAYER')"
              outlined
              :prefix="$t('Bet') + ' x '"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_PAYOUT_PLAYER')"
            />

            <v-text-field
              v-model.number="form.GAME_BACCARAT_PAYOUT_TIE"
              :label="$t('Tie bet')"
              :rules="[validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_PAYOUT_TIE')"
              :error-messages="form.errors.get('GAME_BACCARAT_PAYOUT_TIE')"
              outlined
              :prefix="$t('Bet') + ' x '"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_PAYOUT_TIE')"
            />

            <v-text-field
              v-model.number="form.GAME_BACCARAT_PAYOUT_BANKER"
              :label="$t('Banker bet')"
              :rules="[validationPositiveNumber]"
              :error="form.errors.has('GAME_BACCARAT_PAYOUT_BANKER')"
              :error-messages="form.errors.get('GAME_BACCARAT_PAYOUT_BANKER')"
              outlined
              :prefix="$t('Bet') + ' x '"
              @keydown="clearFormErrors($event, 'GAME_BACCARAT_PAYOUT_BANKER')"
            />
          </v-expansion-panel-content>
        </v-expansion-panel>
        <v-expansion-panel>
          <v-expansion-panel-header>{{ $t('Sounds') }}</v-expansion-panel-header>
          <v-expansion-panel-content>
            <file-upload
              v-model="form.GAME_BACCARAT_SOUNDS_WIN"
              :label="$t('Win')"
              name="win"
              :folder="`games/${packageId}`"
              accept=".webm,.wav,.mp3,.ogg,.m4a,.m4b,.mp4,.aac"
              :clearable="true"
            />

            <file-upload
              v-model="form.GAME_BACCARAT_SOUNDS_LOSE"
              :label="$t('Lose')"
              name="lose"
              :folder="`games/${packageId}`"
              accept=".webm,.wav,.mp3,.ogg,.m4a,.m4b,.mp4,.aac"
              :clearable="true"
            />

            <file-upload
              v-model="form.GAME_BACCARAT_SOUNDS_PUSH"
              :label="$t('Push')"
              name="lose"
              :folder="`games/${packageId}`"
              accept=".webm,.wav,.mp3,.ogg,.m4a,.m4b,.mp4,.aac"
              :clearable="true"
            />
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-card-text>
  </v-card>
</template>

<script>
import { config } from '~/plugins/config'
import FormMixin from '~/mixins/Form'
import FileUpload from '~/components/Admin/FileUpload'

export default {
  components: { FileUpload },
  mixins: [FormMixin],

  props: {
    packageId: {
      type: String,
      required: true
    },
    form: {
      type: Object,
      required: true
    }
  },

  data () {
    return {
      variables: {
        GAME_BACCARAT_CATEGORIES: config(`${this.packageId}.categories`),
        GAME_BACCARAT_BANNER: config(`${this.packageId}.banner`),
        GAME_BACCARAT_MIN_BET: config(`${this.packageId}.min_bet`),
        GAME_BACCARAT_MAX_BET: config(`${this.packageId}.max_bet`),
        GAME_BACCARAT_BET_CHANGE_AMOUNT: config(`${this.packageId}.bet_change_amount`),
        GAME_BACCARAT_DEFAULT_BET_AMOUNT: config(`${this.packageId}.default_bet_amount`),
        GAME_BACCARAT_PAYOUT_PLAYER: config(`${this.packageId}.payouts.player`),
        GAME_BACCARAT_PAYOUT_TIE: config(`${this.packageId}.payouts.tie`),
        GAME_BACCARAT_PAYOUT_BANKER: config(`${this.packageId}.payouts.banker`),
        GAME_BACCARAT_SOUNDS_WIN: config(`${this.packageId}.sounds.win`),
        GAME_BACCARAT_SOUNDS_LOSE: config(`${this.packageId}.sounds.lose`),
        GAME_BACCARAT_SOUNDS_PUSH: config(`${this.packageId}.sounds.push`)
      }
    }
  },

  created () {
    this.$emit('input', this.variables)
  }
}
</script>
