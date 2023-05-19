<template>
  <v-select
    v-model="value"
    :items="options"
    :label="$t('Enabled')"
    :disabled="disabled"
    outlined
    @change="change"
  />
</template>

<script>
export default {
  props: {
    disabled: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data () {
    return {
      value: null,
      options: [
        { text: this.$t('All'), value: null },
        { text: this.$t('Yes'), value: 1 },
        { text: this.$t('No'), value: 0 }
      ]
    }
  },

  methods: {
    change () {
      this.$emit('change', { status: this.value })
    }
  }
}
</script>
