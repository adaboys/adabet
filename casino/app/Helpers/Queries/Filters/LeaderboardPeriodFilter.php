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

class LeaderboardPeriodFilter extends PeriodFilter
{
    public function buildQuery(Builder $query): Builder
    {
        return $query->period($this->getValue(), 'games');
    }
}
