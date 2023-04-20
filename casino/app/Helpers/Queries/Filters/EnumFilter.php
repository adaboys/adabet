<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Helpers\Queries\Filters;

use Illuminate\Database\Eloquent\Builder;

class EnumFilter extends Filter
{
    protected $model;
    protected $table;

    public function buildQuery(Builder $query): Builder
    {
        $const = $this->model . '::' . strtoupper($this->key) . '_' . strtoupper($this->getValue());

        return $query->when(defined($const), function (Builder $query, bool $defined) use ($const) {
            $query->where($this->table . '.' . $this->key, constant($const));
        });
    }
}
