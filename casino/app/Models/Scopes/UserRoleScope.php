<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Models\Scopes;

use App\Models\User;
use Illuminate\Database\Eloquent\Builder;

trait UserRoleScope
{
    public function scopeUserRole(Builder $query, ?array $roles): Builder
    {
        $column = $this->getTable() . '.role';

        return $query->when($roles, function (Builder $query, array $roles) use ($column) {
            $query->whereIn(
                $column,
                collect($roles)->map(function ($role) {
                    return constant(User::class . '::ROLE_' . strtoupper($role));
                })
                
                ->filter()
            );
        });
    }
}
