<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   app/Casts
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

namespace App\Casts;

use Illuminate\Contracts\Database\Eloquent\CastsAttributes;

class UserFields implements CastsAttributes
{
    
    public function get($model, string $key, $value, array $attributes)
    {
        $userFields = collect(is_null($value) ? [] : json_decode($value));

        return collect(config('settings.users.fields'))
            ->mapWithKeys(function ($field) use ($userFields) {
                return [$field->id => $userFields->has($field->id) ? $userFields->get($field->id) : NULL];
            });
    }

    
    public function set($model, string $key, $value, array $attributes)
    {
        return json_encode(is_array($value) ? $value : $value->toArray());
    }
}
