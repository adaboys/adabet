<template>
  <div>
    <v-text-field
      v-if="parameter.type === 'input'"
      :value="value"
      :label="$t(parameter.name)"
      :error="form.errors.has(formKey + '.' + parameter.id)"
      :error-messages="form.errors.get(formKey + '.' + parameter.id)"
      :hint="parameter.description"
      :persistent-hint="!!parameter.description"
      :disabled="disabled"
      :suffix="parameter.suffix || ''"
      outlined
      :class="{ 'mb-5': !!parameter.description }"
      @keydown="clearErrors($event, formKey, parameter.id)"
      @change="change"
    />

    <v-textarea
      v-else-if="parameter.type === 'textarea'"
      :value="value"
      :label="$t(parameter.name)"
      :error="form.errors.has(formKey + '.' + parameter.id)"
      :error-messages="form.errors.get(formKey + '.' + parameter.id)"
      :hint="parameter.description"
      :persistent-hint="!!parameter.description"
      :disabled="disabled"
      outlined
      :class="{ 'mb-5': !!parameter.description }"
      @keydown="clearErrors($event, formKey, parameter.id)"
      @change="change"
    />

    <v-switch
      v-else-if="parameter.type === 'switch'"
      :input-value="value"
      :label="$t(parameter.name)"
      :error="form.errors.has(formKey + '.' + parameter.id)"
      :error-messages="form.errors.get(formKey + '.' + parameter.id)"
      :hint="parameter.description"
      :persistent-hint="!!parameter.description"
      :disabled="disabled"
      outlined
      :class="{ 'mb-5': !!parameter.description }"
      :false-value="0"
      :true-value="1"
      color="primary"
      @change="clearErrors($event, formKey, parameter.id); change($event)"
    />

    <v-select
      v-else-if="parameter.type === 'select'"
      :value="value"
      :items="parameter.options"
      :label="$t(parameter.name)"
      :multiple="parameter.multiple === true"
      :error="form.errors.has(formKey + '.' + parameter.id)"
      :error-messages="form.errors.get(formKey + '.' + parameter.id)"
      :hint="parameter.description"
      :persistent-hint="!!parameter.description"
      :disabled="disabled"
      outlined
      @keydown="clearErrors($event, formKey, parameter.id)"
      @change="change"
    />
  </div>
</template>

<script>
import FormMixin from '~/mixins/Form'

export default {
  name: 'FormParameter',

  mixins: [FormMixin],

  props: {
    parameter: {
      type: Object,
      required: true
    },
    value: {
      validator: value => typeof value === 'string' || typeof value === 'number' || value === null,
      required: true
    },
    form: {
      type: Object,
      required: true
    },
    formKey: {
      type: String,
      required: true,
      default: 'parameters'
    },
    disabled: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  methods: {
    clearErrors (event, formKey, parameterId) {
      this.clearFormErrors(event, formKey + '.' + parameterId)
    },
    change (event) {
      this.$emit('input', event)
    }
  }
}
</script>
