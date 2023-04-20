<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Models;

use App\Helpers\Utils;
use Illuminate\Database\Eloquent\Builder;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Str;

class Asset extends Model
{
    const TYPE_CRYPTO = 1;

    const STATUS_ACTIVE   = 0;
    const STATUS_INACTIVE = 1;

    protected $attributes = [
        'status' => self::STATUS_ACTIVE
    ];

    protected $fillable = ['status', 'price'];

    protected $casts = [
        'type'  => 'integer',
        'price' => 'float'
    ];

    public function predictions()
    {
        return $this->hasMany(AssetPrediction::class);
    }

    public function scopeCrypto($query): Builder
    {
        return $query->where('type', self::TYPE_CRYPTO);
    }

    public function scopeActive($query): Builder
    {
        return $query->where('status', self::STATUS_ACTIVE);
    }

    
    public function getIsActiveAttribute(): bool
    {
        return $this->status == self::STATUS_ACTIVE;
    }

    
    public function getTypeNameAttribute(): string
    {
        
        return Str::of(Utils::getConstantNameByValue(__CLASS__, $this, $this->type))->replace('TYPE_', '')->lower()->ucfirst();
    }
}
