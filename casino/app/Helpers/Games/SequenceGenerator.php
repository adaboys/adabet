<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   SequenceGenerator.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Helpers\Games;

use Exception;
use Illuminate\Support\Collection;

class SequenceGenerator
{
    private $size;
    private $min;
    private $max;
    private $sequence;

    public function __construct(int $size, int $min, int $max)
    {
        if ($size > ($max - $min + 1)) {
            throw new Exception('Too large sequence requested.');
        }

        $this->size = $size;
        $this->min = $min;
        $this->max = $max;
        $this->sequence = collect();

        return $this;
    }

    
    public function generateUnique(): SequenceGenerator
    {
        for ($i = 0; $i < $this->size; $i++) {
            do {
                $ng = (new NumberGenerator($this->min, $this->max))->generate();
            } while ($this->sequence->contains(function (NumberGenerator $item) use ($ng) {
                return $item->getNumber() == $ng->getNumber();
            }));

            $this->sequence->push($ng);
        }

        return $this;
    }

    
    public function shift(int $shift): SequenceGenerator
    {
        $this->sequence->each(function (NumberGenerator $ng) use ($shift) {
            $ng->shift($shift);
        });

        return $this;
    }

    
    public function get(): Collection
    {
        return $this->sequence->map(function (NumberGenerator $ng) {
            return $ng->getNumber();
        });
    }

    
    public function set(Collection $sequence): SequenceGenerator
    {
        $this->sequence = $sequence;

        $this->sequence->transform(function ($n) {
            return (new NumberGenerator($this->min, $this->max))->setNumber($n);
        });

        return $this;
    }
}
