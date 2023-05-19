<template>
  <v-container fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4">
        <v-card>
          <v-toolbar>
            <v-toolbar-title>
              Deposit
            </v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form v-model="formIsValid" class="mt-3" @submit.prevent="deposit">
              <v-text-field 
                :label="$t('Deposit amount')" 
                :suffix="$t('credits')" 
                :error="form.errors.has('amount')" 
                :error-messages="form.errors.get('amount')"
                :value="!!parseInt(form.amount)"
                v-model="form.amount"
                outlined>
                <template v-slot:append-outer>
                  <v-skeleton-loader type="button" :loading="allowed === null">
                    <v-btn type="submit" color="primary" class="mt-n3" large :disabled="!formIsValid || form.busy"
                      :loading="form.busy">
                      Deposit
                    </v-btn>
                  </v-skeleton-loader>
                </template>
              </v-text-field>
            </v-form>

          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import Form from 'vform'
import FormMixin from '~/mixins/Form'
import { config } from '~/plugins/config'
import CountdownTimer from '~/components/CountdownTimer'
import {mapState} from "vuex";

export default {
  components: { CountdownTimer },

  mixins: [FormMixin],

  middleware: ['auth', 'verified', '2fa_passed'],

  metaInfo() {
    return { title: this.$t('Deposit') }
  },

  data() {
    return {
      allowed: null,
      time: null,
      form: new Form({
        amount: '0'
      }),
      url: '/api/user/deposit'
    }
  },

  computed: {
    ...mapState('auth', ['account']),

    adaToChip() {
      return parseInt(config('wallet.ada_to_chip'))
    }
  },
  async created() {
    this.checkFaucet()
  },

  methods: {
    async checkFaucet() {
      this.allowed = true
    },

    async deposit() {
      //const { data: { success } } = await this.form.submit('post', this.url)
      console.log(this.adaToChip)
      // if (success) {
      //   this.allowed = false
      //   this.account.balance += (this.form.amount * this.adaToChip)
      //   this.form.amount = 0
      //   this.$store.dispatch('message/success', { text: this.$t('{0} credits added to your account.', [this.amount]) })
      // }
    }
  }
}
</script>
