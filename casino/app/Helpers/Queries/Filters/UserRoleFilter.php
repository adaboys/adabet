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

use App\Models\User;
use Illuminate\Database\Eloquent\Builder;

class UserRoleFilter extends Filter
{
    protected $key = 'roles';

    public function buildQuery(Builder $query): Builder
    {
        return $query->whereIn(
            'users.role',
            collect($this->getValue())
                ->map(function ($role) { return constant(User::class . '::ROLE_' . strtoupper($role)); })
                ->filter() 
        );
    }
}
