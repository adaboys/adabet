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

class UserStatusFilter extends EnumFilter
{
    protected $key = 'status';
    protected $model = User::class;
    protected $table = 'users';
}
