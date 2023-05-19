<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   AffiliateCommissionStatusFilter.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Helpers\Queries\Filters;

use App\Models\AffiliateCommission;

class AffiliateCommissionStatusFilter extends EnumFilter
{
    protected $key = 'status';
    protected $model = AffiliateCommission::class;
    protected $table = 'affiliate_commissions';
}
