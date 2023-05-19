<template>
  <v-container>
    <v-row align="center" justify="center">
      <v-col cols="12" md="6">
        <v-card>
          <v-toolbar>
            <v-btn icon @click="$router.go(-1)">
              <v-icon>mdi-arrow-left</v-icon>
            </v-btn>
            <v-toolbar-title>
              {{ $t('Register bundle') }}
            </v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form v-model="formIsValid" @submit.prevent="submit">
              <v-text-field
                v-model="form.code"
                :label="$t('Purchase code')"
                :disabled="form.busy"
                :rules="[validationRequired]"
                :error="form.errors.has('code')"
                :error-messages="form.errors.get('code')"
                outlined
                @keydown="clearFormErrors($event,'code')"
              />

              <v-btn type="submit" color="primary" :disabled="!formIsValid || form.busy" :loading="form.busy">
                {{ $t('Register') }}
              </v-btn>
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

export default {
  mixins: [FormMixin],

  middleware: ['auth', 'verified', '2fa_passed', 'admin'],

  metaInfo () {
    return { title: this.$t('Register bundle', [this.id]) }
  },

  data () {
    return {
      data: null,
      form: new Form({
        code: null
      })
    }
  },

  methods: {
    async submit () {
      const { data } = await this.form.post('/api/admin/add-ons/register-bundle')

      this.$store.dispatch('message/' + (data.success ? 'success' : 'error'), { text: data.message })

      if (data.success) {
        this.$router.push({ name: 'admin.add-ons.index' })
      }
    }
  }
}
</script>
